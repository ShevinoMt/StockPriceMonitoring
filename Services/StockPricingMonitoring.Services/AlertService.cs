using Microsoft.Extensions.Logging;
using StockPriceMonitoring.Core.Enumerations;
using StockPriceMonitoring.Core.Models;
using StockPricingMonitoring.Repositories.Core;
using StockPricingMonitoring.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPricingMonitoring.Services
{
    public class AlertService : IAlertService
    {
        private readonly IPriceAlertRepository _alertRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AlertService> _logger;

        public AlertService(
            IPriceAlertRepository alertRepository,
            INotificationService notificationService,
            ILogger<AlertService> logger)
        {
            _alertRepository = alertRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<PriceAlert> CreateAlertAsync(string userId, string symbol, decimal threshold, AlertType type)
        {
            var alert = new PriceAlert
            {
                UserId = userId,
                Symbol = symbol.ToUpper(),
                Threshold = threshold,
                Type = type,
                Status = AlertStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            return await _alertRepository.CreateAlertAsync(alert);
        }

        public async Task<IEnumerable<PriceAlert>> GetUserAlertsAsync(string userId)
        {
            return await _alertRepository.GetUserAlertsAsync(userId);
        }

        public async Task DeleteAlertAsync(int alertId, string userId)
        {
            var alert = await _alertRepository.GetByIdAsync(alertId);
            if (alert != null && alert.UserId == userId)
            {
                await _alertRepository.DeleteAlertAsync(alertId);
            }
        }

        public async Task CheckAlertsAsync(IEnumerable<StockPrice> currentPrices)
        {
            var priceDict = currentPrices.ToDictionary(p => p.Symbol, p => p);
            var activeAlerts = await _alertRepository.GetActiveAlertsAsync();

            var triggeredAlerts = new List<PriceAlert>();

            foreach (var alert in activeAlerts)
            {
                if (!priceDict.TryGetValue(alert.Symbol, out var currentPrice))
                    continue;

                bool isTriggered;
                if (alert.Type == AlertType.Above)
                    isTriggered = currentPrice.Price >= alert.Threshold;
                else if (alert.Type == AlertType.Below)
                    isTriggered = currentPrice.Price <= alert.Threshold;
                else
                    isTriggered = false;

                if (isTriggered)
                {
                    alert.Status = AlertStatus.Triggered;
                    alert.TriggeredAt = DateTime.UtcNow;
                    alert.TriggeredPrice = currentPrice.Price;

                    triggeredAlerts.Add(alert);

                    var notification = new AlertNotification
                    {
                        AlertId = alert.Id,
                        UserId = alert.UserId,
                        Symbol = alert.Symbol,
                        Threshold = alert.Threshold,
                        Type = alert.Type,
                        CurrentPrice = currentPrice.Price,
                        Timestamp = DateTime.UtcNow,
                        Message = $"{alert.Symbol} is now {(alert.Type == AlertType.Above ? "above" : "below")} ${alert.Threshold:F2} at ${currentPrice.Price:F2}"
                    };

                    await _notificationService.SendAlertNotificationAsync(notification);
                }
            }

            // Update triggered alerts in batch
            foreach (var alert in triggeredAlerts)
            {
                await _alertRepository.UpdateAlertAsync(alert);
            }

            if (triggeredAlerts.Any())
            {
                _logger.LogInformation("Triggered {Count} alerts", triggeredAlerts.Count);
            }
        }
    }
}

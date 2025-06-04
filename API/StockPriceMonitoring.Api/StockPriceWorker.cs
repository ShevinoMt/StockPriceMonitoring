using StockPricingMonitoring.Repositories.Core;
using StockPricingMonitoring.Services.Core;
using StockPricingMonitoring.Services.Core.Interfaces;

namespace StockPriceMonitoring.Api
{
    public class StockPriceWorker : BackgroundService
    {
        private readonly ILogger<StockPriceWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan interval = TimeSpan.FromSeconds(15);

        public StockPriceWorker(IServiceProvider serviceProvider, 
            ILogger<StockPriceWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stock Price Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateStockPricesAsync(stoppingToken);
                    await Task.Delay(interval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating stock prices");
                    await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
                }
            }
        }

        private async Task UpdateStockPricesAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var stockPriceService = scope.ServiceProvider.GetRequiredService<IStockPriceService>();
            var stockPriceRepository = scope.ServiceProvider.GetRequiredService<IStockPriceRepository>();
            var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var cacheService = scope.ServiceProvider.GetRequiredService<IStockPriceCacheService>();

            //Simulate new stock prices
            var newPrices = await stockPriceService.SimulatePricesAsync(cancellationToken);

            // Save to database
            await stockPriceRepository.SavePricesAsync(newPrices, cancellationToken);

            // Update cache
            foreach (var price in newPrices)
            {
                var cacheKey = $"stock_price_{price.Symbol}";
                await cacheService.SetAsync(cacheKey, price, TimeSpan.FromSeconds(30));

                // Send real-time price updates
                await notificationService.SendPriceUpdateAsync(price);
            }

            // Check alerts
            await alertService.CheckAlertsAsync(newPrices);

            _logger.LogDebug("Updated prices for {Count} symbols", newPrices.Count());
        }
    }
}

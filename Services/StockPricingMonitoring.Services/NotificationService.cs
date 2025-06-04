using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using StockPriceMonitoring.Core.Models;
using StockPricingMonitoring.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPricingMonitoring.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        /// <summary>
        /// Sends an alert notification to a specific user.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task SendAlertNotificationAsync(AlertNotification notification)
        {
            //await hubContext.Clients.User(notification.UserId)
            //    .SendAsync("AlertTriggered", notification);
            await hubContext.Clients.Group($"User_{notification.UserId}")
                    .SendAsync("AlertTriggered", notification);
        }

        /// <summary>
        /// Sends a stock price update to all connected clients.
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public async Task SendPriceUpdateAsync(StockPrice price)
        {
            await hubContext.Clients.All
                .SendAsync("PriceUpdate", price);
        }
    }

    // SignalR Hub
    public class NotificationHub : Hub
    {
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            
        }

        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
        }
    }
}

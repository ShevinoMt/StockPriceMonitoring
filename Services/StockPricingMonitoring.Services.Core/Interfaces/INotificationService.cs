using StockPriceMonitoring.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPricingMonitoring.Services.Core.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Sends an alert notification to a specific user.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task SendAlertNotificationAsync(AlertNotification notification);

        /// <summary>
        /// Send a stock price update to all connected clients.
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        Task SendPriceUpdateAsync(StockPrice price);
    }
}

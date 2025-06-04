using StockPriceMonitoring.Core.Enumerations;
using StockPriceMonitoring.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPricingMonitoring.Services.Core.Interfaces
{
    public interface IAlertService
    {
        Task<PriceAlert> CreateAlertAsync(string userId, string symbol, decimal threshold, AlertType type);
        Task<IEnumerable<PriceAlert>> GetUserAlertsAsync(string userId);
        Task DeleteAlertAsync(int alertId, string userId);
        Task CheckAlertsAsync(IEnumerable<StockPrice> currentPrices);
    }
}

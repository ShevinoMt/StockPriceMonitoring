using StockPriceMonitoring.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPricingMonitoring.Repositories.Core
{
    public interface IPriceAlertRepository
    {
        Task<PriceAlert?> GetByIdAsync(int id);
        Task<IEnumerable<PriceAlert>> GetActiveAlertsAsync();
        Task<IEnumerable<PriceAlert>> GetActiveAlertsBySymbolAsync(string symbol);
        Task<IEnumerable<PriceAlert>> GetUserAlertsAsync(string userId);
        Task<PriceAlert> CreateAlertAsync(PriceAlert alert);
        Task UpdateAlertAsync(PriceAlert alert);
        Task DeleteAlertAsync(int id);
    }
}

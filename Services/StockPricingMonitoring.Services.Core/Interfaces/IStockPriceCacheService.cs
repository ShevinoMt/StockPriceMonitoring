using StockPriceMonitoring.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPricingMonitoring.Services.Core.Interfaces
{
    public interface IStockPriceCacheService
    {
        Task<StockPrice?> GetAsync(string key, CancellationToken cancellationToken);
        Task SetAsync(string key, StockPrice value, TimeSpan? expiration = null,
            CancellationToken cancellationToken = default);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }
}

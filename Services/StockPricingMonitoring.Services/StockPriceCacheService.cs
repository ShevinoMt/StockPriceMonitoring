using MemoryPack;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StockPriceMonitoring.Core.Models;
using StockPricingMonitoring.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockPricingMonitoring.Services
{
    public class StockPriceCacheService : IStockPriceCacheService
    {
        private readonly IDistributedCache cache;
        // We are using MemoryPack, which is more compact and faster for serialization.
        private readonly ILogger<StockPriceCacheService> logger;

        public StockPriceCacheService(IDistributedCache cache, ILogger<StockPriceCacheService> logger)
        {
            this.cache = cache;            
            this.logger = logger;
        }

        /// <summary>
        /// Retrieves a stock price from the cache by its key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<StockPrice?> GetAsync(string key, CancellationToken cancellationToken) 
        {
            var cachedValue = await cache.GetAsync(key, cancellationToken);
            if (cachedValue !=null)
            {
                logger.LogDebug("Retrieved from cache for key: {Key}", key);
                return MemoryPackSerializer.Deserialize<StockPrice>(cachedValue);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets a stock price in the cache with an optional expiration time.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SetAsync(string key, StockPrice value, 
            TimeSpan? expiration = null, CancellationToken cancellationToken = default) 
        {
            var options = new DistributedCacheEntryOptions();
            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);

            await cache.SetAsync(key, MemoryPackSerializer.Serialize(value), options);
        }

        /// <summary>
        /// Removes a stock price from the cache by its key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string key)
        {
            await cache.RemoveAsync(key);
        }

        /// <summary>
        /// Checks if a stock price exists in the cache by its key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string key)
        {
            var value = await cache.GetStringAsync(key);
            return value != null;
        }
    }
}

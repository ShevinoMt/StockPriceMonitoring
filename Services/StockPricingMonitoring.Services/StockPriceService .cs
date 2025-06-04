using StockPriceMonitoring.Core.Models;
using StockPricingMonitoring.Repositories.Core;
using StockPricingMonitoring.Services.Core.Interfaces;

namespace StockPricingMonitoring.Services
{
    public class StockPriceService : IStockPriceService
    {
        private readonly IStockPriceRepository _repository;
        private readonly IStockPriceCacheService _cache;
        private readonly Random _random;

        // Base prices for simulation
        private readonly Dictionary<string, decimal> basePrices = new()
        {
            { "AAPL", 180.00m },
            { "GOOGL", 140.00m },
            { "MSFT", 380.00m },
            { "TSLA", 250.00m },
            { "AMZN", 145.00m }
        };

        public StockPriceService(IStockPriceRepository repository, IStockPriceCacheService cache)
        {
            _repository = repository;
            _cache = cache;
            _random = new Random();
        }

        public IEnumerable<string> GetSupportedStockSymbols() => basePrices.Keys;

        /// <summary>
        /// Gets the current stock price for a given symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<StockPrice> GetCurrentPriceAsync(string symbol, CancellationToken cancellationToken)
        {
            // Try cache first
             var cacheKey = $"stock_price_{symbol}";
            var cachedPrice = await _cache.GetAsync(cacheKey, cancellationToken);

            if (cachedPrice != null)
                return cachedPrice;

            // Get from database
            var latestPrice = await _repository.GetLatestPriceAsync(symbol, cancellationToken);

            if (latestPrice != null)
            {
                await _cache.SetAsync(cacheKey, latestPrice, TimeSpan.FromSeconds(30));
                return latestPrice;
            }

            // Generate initial price if none exists
            var basePrice = basePrices.GetValueOrDefault(symbol, 100.00m);
            var price = new StockPrice
            {
                Symbol = symbol,
                Price = basePrice,
                Timestamp = DateTime.UtcNow,
                DayOpen = basePrice,
                DayHigh = basePrice,
                DayLow = basePrice
            };

            await _repository.SavePriceAsync(price, cancellationToken);
            return price;
        }

        /// <summary>
        /// Get current stock prices for supported symbols.
        /// </summary>
        /// <param name="sybols"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StockPrice>> GetCurrentPricesAsync(IEnumerable<string> sybols, 
            CancellationToken cancellationToken)
        {
            //We are not using Task.WhenAll here as the EF DbContext is not thread safe 
            var prices = new List<StockPrice>();
            foreach (var symbol in sybols)
            {
                prices.Add(await GetCurrentPriceAsync(symbol, cancellationToken));
            }
            return prices;
        }

        /// <summary>
        /// Simulate price changes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StockPrice>> SimulatePricesAsync(CancellationToken cancellationToken)
        {
            var prices = new List<StockPrice>();
            var symbols = GetSupportedStockSymbols().ToList();  

            foreach (var symbol in symbols)
            {
                var currentPrice = await GetCurrentPriceAsync(symbol, cancellationToken);
                var newPrice = SimulatePriceMovement(currentPrice);
                prices.Add(newPrice);
            }

            return prices;
        }

        /// <summary>
        /// Simulate price movement for a stock based on a random percentage change.
        /// </summary>
        /// <param name="currentPrice"></param>
        /// <returns></returns>
        private StockPrice SimulatePriceMovement(StockPrice currentPrice)
        {
            // Simulate price movement from -2% to +2%
            var changePercent = _random.Next(-2, 3) / 100.0; // -2% to +2%
            var priceChange = currentPrice.Price * (decimal)changePercent;
            var newPrice = Math.Max(0.01m, currentPrice.Price + priceChange); //This is to ensure price doesn't go down to 0

            return new StockPrice
            {
                Symbol = currentPrice.Symbol,
                Price = Math.Round(newPrice, 2),
                Timestamp = DateTime.UtcNow,
                DayOpen = currentPrice.DayOpen,
                DayHigh = Math.Max(currentPrice.DayHigh, newPrice),
                DayLow = Math.Min(currentPrice.DayLow, newPrice),
            };
        }
        
    }
}

using StockPriceMonitoring.Core.Models;

namespace StockPricingMonitoring.Repositories.Core
{
    public interface IStockPriceRepository
    {
        /// <summary>
        /// Get the latest stock price for a given symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<StockPrice?> GetLatestPriceAsync(string symbol, CancellationToken cancellationToken);

        /// <summary>
        /// Get the latest stock prices for a list of symbols.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<StockPrice>> GetLatestPricesAsync(IEnumerable<string> symbols, CancellationToken cancellationToken);

        /// <summary>
        /// Get the price history for a given stock symbol within a date range.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<StockPrice>> GetPriceHistoryAsync(string symbol, DateTime from, DateTime to, 
            CancellationToken cancellationToken);

        /// <summary>
        /// Save a single stock price record to the database.
        /// </summary>
        /// <param name="stockPrice"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SavePriceAsync(StockPrice stockPrice, CancellationToken cancellationToken);

        /// <summary>
        /// Save multiple stock price records to the database.
        /// </summary>
        /// <param name="stockPrices"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SavePricesAsync(IEnumerable<StockPrice> stockPrices, CancellationToken cancellationToken);
    }
}

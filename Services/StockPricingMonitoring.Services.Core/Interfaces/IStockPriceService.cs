using StockPriceMonitoring.Core.Models;

namespace StockPricingMonitoring.Services.Core.Interfaces
{
    public interface IStockPriceService
    {
        /// <summary>
        /// Get the list of supported stock symbols.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetSupportedStockSymbols();

        /// <summary>
        /// Get the current stock price for a given symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<StockPrice> GetCurrentPriceAsync(string symbol, CancellationToken cancellationToken);

        /// <summary>
        /// Get current stock prices for supported symbols.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<StockPrice>> GetCurrentPricesAsync(IEnumerable<string> symbols,
            CancellationToken cancellationToken);
        
        /// <summary>
        /// Simulate price changes
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<StockPrice>> SimulatePricesAsync(CancellationToken cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using StockPriceMonitoring.Core.Models;
using StockPricingMonitoring.Repositories.Core;
using StockPricingMonitoring.Repositories.EF.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPricingMonitoring.Repositories.EF
{
    public class StockPriceRepository : IStockPriceRepository
    {
        private readonly ApplicationDbContext _context;

        public StockPriceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the latest stock price for a given symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<StockPrice?> GetLatestPriceAsync(string symbol, CancellationToken cancellationToken)
        {
            return await _context.StockPrices
                .Where(sp => sp.Symbol == symbol)
                .OrderByDescending(sp => sp.Timestamp)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the latest stock prices for multiple symbols.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StockPrice>> GetLatestPricesAsync(IEnumerable<string> symbols,
            CancellationToken cancellationToken)
        {
            var result = new List<StockPrice>();

            foreach (var symbol in symbols)
            {
                var latest = await GetLatestPriceAsync(symbol, cancellationToken);
                if (latest != null)
                    result.Add(latest);
            }

            return result;
        }

        /// <summary>
        /// Gets the price history for a given stock symbol within a date range.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StockPrice>> GetPriceHistoryAsync(string symbol, DateTime from,
            DateTime to, CancellationToken cancellationToken)
        {
            return await _context.StockPrices
                .Where(sp => sp.Symbol == symbol && sp.Timestamp >= from && sp.Timestamp <= to)
                .OrderBy(sp => sp.Timestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Saves a new stock price to the database.
        /// </summary>
        /// <param name="stockPrice"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SavePriceAsync(StockPrice stockPrice, CancellationToken cancellationToken)
        {
            _context.StockPrices.Add(stockPrice);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Saves multiple stock prices to the database.
        /// </summary>
        /// <param name="stockPrices"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SavePricesAsync(IEnumerable<StockPrice> stockPrices, CancellationToken cancellationToken)
        {
            _context.StockPrices.AddRange(stockPrices);
            await _context.SaveChangesAsync();
        }
    }
}

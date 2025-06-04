using Moq;
using Xunit;
using StockPriceMonitoring.Core.Models;
using StockPricingMonitoring.Services.Core.Interfaces;
using StockPricingMonitoring.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockPricingMonitoring.Repositories.EF.Infra;
using StockPricingMonitoring.Repositories.EF;
using Microsoft.EntityFrameworkCore;

namespace StockPricingMonitoring.Tests
{
    public class StockPriceServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly StockPriceRepository repository;
        private readonly Mock<IStockPriceCacheService> mockCache;
        private readonly StockPriceService stockPriceService;

        public StockPriceServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            repository = new StockPriceRepository(_context);
            mockCache = new Mock<IStockPriceCacheService>();
            stockPriceService = new StockPriceService(repository, mockCache.Object);
        }

        [Fact]
        public async Task GetCurrentPriceAsync_ExistingSymbol_ReturnsPrice()
        {
            var cancellationToken = CancellationToken.None;
            
            var symbol = "AAPL";
            var expectedPrice = new StockPrice
            {
                Symbol = symbol,
                Price = 180.00m,
                Timestamp = DateTime.UtcNow
            };

            _context.StockPrices.Add(expectedPrice);
            await _context.SaveChangesAsync();

            mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync((StockPrice?)null);

           
            var result = await stockPriceService.GetCurrentPriceAsync(symbol, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(symbol, result.Symbol);
            Assert.True(result.Price > 0);
        }

        [Fact]
        public async Task SimulatePricesAsync_ValidSymbols_ReturnsUpdatedPrices()
        {
            var cancellationToken = CancellationToken.None;
            
            var symbols = stockPriceService.GetSupportedStockSymbols().ToList();    

            foreach (var symbol in symbols)
            {
                var price = new StockPrice
                {
                    Symbol = symbol,
                    Price = 100.00m,
                    Timestamp = DateTime.UtcNow.AddMinutes(-1),
                    DayOpen = 100.00m,
                    DayHigh = 100.00m,
                    DayLow = 100.00m,
                };
                _context.StockPrices.Add(price);
            }
            await _context.SaveChangesAsync();

            mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync((StockPrice?)null);

            
            var results = await stockPriceService.SimulatePricesAsync(cancellationToken);

            // Assert
            Assert.Equal(symbols.Count, results.Count());
            Assert.All(results, price => Assert.True(price.Price > 0));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

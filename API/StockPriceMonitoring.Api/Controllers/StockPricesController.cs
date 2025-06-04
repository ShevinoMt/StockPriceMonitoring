using Microsoft.AspNetCore.Mvc;
using StockPriceMonitoring.Core.Models;
using StockPricingMonitoring.Services;
using StockPricingMonitoring.Services.Core.Interfaces;

namespace StockPriceMonitoring.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockPricesController : ControllerBase
    {
        private readonly IStockPriceService stockPriceService;
        private readonly ILogger<StockPricesController> logger;

        public StockPricesController(IStockPriceService stockPriceService,
            ILogger<StockPricesController> logger)
        {
            this.stockPriceService = stockPriceService;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the current stock price for a given symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{symbol}")]
        public async Task<ActionResult<StockPrice>> GetCurrentPrice(string symbol,
            CancellationToken cancellationToken)
        {
            try
            {
                var price = await stockPriceService.GetCurrentPriceAsync(symbol.ToUpper(), cancellationToken);
                return Ok(price);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting current price for symbol {Symbol}", symbol);
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets the current stock prices for multiple symbols.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockPrice>>> GetCurrentPrices([FromQuery] string[] symbols,
            CancellationToken cancellationToken)
        {
            try
            {
                var symbolsToQuery = symbols.Any() ? symbols : stockPriceService.GetSupportedStockSymbols().ToArray();
                var prices = await stockPriceService.GetCurrentPricesAsync(symbolsToQuery.Select(s => s.ToUpper()), cancellationToken);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting current prices for symbols {Symbols}", string.Join(", ", symbols));  
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

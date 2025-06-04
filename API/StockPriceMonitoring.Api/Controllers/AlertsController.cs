using Microsoft.AspNetCore.Mvc;
using StockPriceMonitoring.Api.Models;
using StockPriceMonitoring.Core.Models;
using StockPricingMonitoring.Services.Core.Interfaces;

namespace StockPriceMonitoring.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertService _alertService;
        private readonly ILogger<AlertsController> logger;

        public AlertsController(IAlertService alertService,
            ILogger<AlertsController> logger)
        {
            _alertService = alertService;
            this.logger = logger;   
        }

        /// <summary>
        /// Create a new price alert for a user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<PriceAlert>> CreateAlert([FromBody] CreateAlertRequest request)
        {
            try
            {
                // In a real app, get userId from authentication context
                var userId = request.UserId ?? "default_user";

                var alert = await _alertService.CreateAlertAsync(
                    userId,
                    request.Symbol,
                    request.Threshold,
                    request.Type);

                return CreatedAtAction(nameof(GetUserAlerts), new { userId }, alert);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating price alert for user {UserId}", request.UserId);
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get all price alerts for user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PriceAlert>>> GetUserAlerts(string userId)
        {
            try
            {
                var alerts = await _alertService.GetUserAlertsAsync(userId);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting alerts for user {UserId}", userId);
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a price alert for user.
        /// </summary>
        /// <param name="alertId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{alertId}/user/{userId}")]
        public async Task<ActionResult> DeleteAlert(int alertId, string userId)
        {
            try
            {
                await _alertService.DeleteAlertAsync(alertId, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting alert {AlertId} for user {UserId}", alertId, userId);
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

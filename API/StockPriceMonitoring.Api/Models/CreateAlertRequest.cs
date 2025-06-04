using StockPriceMonitoring.Core.Enumerations;

namespace StockPriceMonitoring.Api.Models
{
    public class CreateAlertRequest
    {

        public string? UserId { get; set; }

        /// <summary>
        /// Stock symbol or name example: AAPL, MSFT, GOOGL.
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Threshold value for the alert. This is the price at which the alert will be triggered.
        /// </summary>
        public decimal Threshold { get; set; }

        /// <summary>
        /// Type of alert to create. This indicates whether the alert is for a price above or below the threshold.
        /// </summary>
        public AlertType Type { get; set; }
    }
}

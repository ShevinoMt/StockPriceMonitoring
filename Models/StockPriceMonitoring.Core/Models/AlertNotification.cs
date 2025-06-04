using StockPriceMonitoring.Core.Enumerations;

namespace StockPriceMonitoring.Core.Models
{

    public class AlertNotification
    {
        public int AlertId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public decimal Threshold { get; set; }
        public AlertType Type { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

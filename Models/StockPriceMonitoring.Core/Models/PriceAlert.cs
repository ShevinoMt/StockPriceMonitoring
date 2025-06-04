using StockPriceMonitoring.Core.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace StockPriceMonitoring.Core.Models
{
    public class PriceAlert
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Symbol { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Threshold { get; set; }

        public AlertType Type { get; set; }
        public AlertStatus Status { get; set; } = AlertStatus.Active;

        public DateTime CreatedAt { get; set; }
        public DateTime? TriggeredAt { get; set; }

        public decimal? TriggeredPrice { get; set; }
    }
}

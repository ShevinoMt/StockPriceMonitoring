using MemoryPack;
using System.ComponentModel.DataAnnotations;

namespace StockPriceMonitoring.Core.Models
{

    [MemoryPackable]
    public partial class StockPrice //Partial class is required by MemoryPack for serialization
    {
        public int Id { get; set; }

        /// <summary>
        /// Stock symbol or name example: AAPL, MSFT, GOOGL.
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Symbol { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal DayOpen { get; set; }
        public decimal DayHigh { get; set; }
        public decimal DayLow { get; set; }
        
    }
}
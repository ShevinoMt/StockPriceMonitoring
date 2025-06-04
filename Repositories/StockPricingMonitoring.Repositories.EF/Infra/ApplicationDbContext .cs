using Microsoft.EntityFrameworkCore;
using StockPriceMonitoring.Core.Models;

namespace StockPricingMonitoring.Repositories.EF.Infra
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<StockPrice> StockPrices { get; set; }
        public DbSet<PriceAlert> PriceAlerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StockPrice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Symbol).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Price).HasPrecision(18, 4);
                entity.Property(e => e.DayOpen).HasPrecision(18, 4);
                entity.Property(e => e.DayHigh).HasPrecision(18, 4);
                entity.Property(e => e.DayLow).HasPrecision(18, 4);
                entity.HasIndex(e => new { e.Symbol, e.Timestamp });
            });

            modelBuilder.Entity<PriceAlert>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Symbol).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Threshold).HasPrecision(18, 4);
                entity.Property(e => e.TriggeredPrice).HasPrecision(18, 4);
                entity.HasIndex(e => new { e.Symbol, e.Status });
                entity.HasIndex(e => e.UserId);
            });
        }
    }
}

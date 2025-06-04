using Microsoft.EntityFrameworkCore;
using StockPriceMonitoring.Core.Enumerations;
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
    public class PriceAlertRepository : IPriceAlertRepository
    {
        private readonly ApplicationDbContext _context;

        public PriceAlertRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PriceAlert?> GetByIdAsync(int id)
        {
            return await _context.PriceAlerts.FindAsync(id);
        }

        public async Task<IEnumerable<PriceAlert>> GetActiveAlertsAsync()
        {
            return await _context.PriceAlerts
                .Where(pa => pa.Status == AlertStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<PriceAlert>> GetActiveAlertsBySymbolAsync(string symbol)
        {
            return await _context.PriceAlerts
                .Where(pa => pa.Symbol == symbol && pa.Status == AlertStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<PriceAlert>> GetUserAlertsAsync(string userId)
        {
            return await _context.PriceAlerts
                .Where(pa => pa.UserId == userId)
                .OrderByDescending(pa => pa.CreatedAt)
                .ToListAsync();
        }

        public async Task<PriceAlert> CreateAlertAsync(PriceAlert alert)
        {
            _context.PriceAlerts.Add(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task UpdateAlertAsync(PriceAlert alert)
        {
            _context.PriceAlerts.Update(alert);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAlertAsync(int id)
        {
            var alert = await _context.PriceAlerts.FindAsync(id);
            if (alert != null)
            {
                _context.PriceAlerts.Remove(alert);
                await _context.SaveChangesAsync();
            }
        }
    }
}

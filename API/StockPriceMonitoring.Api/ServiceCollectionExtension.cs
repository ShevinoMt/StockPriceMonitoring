using Microsoft.EntityFrameworkCore;
using StockPricingMonitoring.Repositories.Core;
using StockPricingMonitoring.Repositories.EF;
using StockPricingMonitoring.Repositories.EF.Infra;
using StockPricingMonitoring.Services;
using StockPricingMonitoring.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceMonitoring.Api
{
    /// <summary>
    /// Extension methods for configuring services in the Stock Pricing Monitoring API.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection PopulateApiServices(this IServiceCollection services,
            ConfigurationManager configurationManager)
        {
            services.AddScoped<IStockPriceService, StockPriceService>();
            services.AddScoped<IStockPriceRepository, StockPriceRepository>();
            services.AddScoped<IPriceAlertRepository, PriceAlertRepository>();

            //services.AddDistributedMemoryCache();
            services.AddScoped<IStockPriceCacheService, StockPriceCacheService>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<INotificationService, NotificationService>();

            var test = configurationManager.GetConnectionString("DBConnection");
            Console.WriteLine($"MySql Connection: {configurationManager.GetConnectionString("DBConnection")}");
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                configurationManager.GetConnectionString("DBConnection"),
                ServerVersion.AutoDetect(configurationManager.GetConnectionString("DBConnection"))),
                ServiceLifetime.Scoped);

            // SignalR
            services.AddSignalR(); // Ensure the Microsoft.AspNetCore.SignalR package is installed

            // Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configurationManager.GetConnectionString("RedisConnection");
            });

            return services;
        }
    }
}

using StockPricingMonitoring.Repositories.Core;
using StockPricingMonitoring.Services.Core.Interfaces;
using StockPricingMonitoring.Services;
using StockPricingMonitoring.Repositories.EF.Infra;
using StockPricingMonitoring.Repositories.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace StockPriceMonitoring.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            
            builder.Services.PopulateApiServices(builder.Configuration);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Andre Cassar - Stock Monitoring API",
                    Description = "Stock Monitoring API",
                    Contact = new OpenApiContact
                    {
                        Name = "Andre Cassar",
                        Email = "andre@andrecassar.com",
                        
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                    }
                });

                foreach (var name in Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.AllDirectories))
                {
                    options.IncludeXmlComments(name);
                }


            }).AddSwaggerGenNewtonsoftSupport();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Background Services
            builder.Services.AddHostedService<StockPriceWorker>();

            // Logging
            builder.Services.AddLogging();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            // Ensure database is created
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
            }

            // Map the SignalR hub endpoint
            app.MapHub<NotificationHub>("/notificationhub");

            app.Run();
        }
    }
}

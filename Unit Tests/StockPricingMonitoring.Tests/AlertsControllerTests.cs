using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StockPriceMonitoring.Api.Controllers;
using StockPriceMonitoring.Api.Models;
using StockPriceMonitoring.Core.Enumerations;
using StockPriceMonitoring.Core.Models;
using StockPricingMonitoring.Services;
using StockPricingMonitoring.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPricingMonitoring.Tests
{
    public class AlertsControllerTests
    {
        private readonly Mock<IAlertService> mockAlertService;
        private readonly AlertsController controller;
        private readonly Mock<ILogger<AlertsController>> mockLogger;

        public AlertsControllerTests()
        {
            mockAlertService = new Mock<IAlertService>();
            mockLogger = new Mock<ILogger<AlertsController>>();
            controller = new AlertsController(mockAlertService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task CreateAlert_ValidRequest_ReturnsCreatedResult()
        {
            // Arrange
            var request = new CreateAlertRequest
            {
                UserId = "testuser",
                Symbol = "AAPL",
                Threshold = 180.00m,
                Type = AlertType.Above
            };

            var expectedAlert = new PriceAlert
            {
                Id = 1,
                UserId = request.UserId,
                Symbol = request.Symbol,
                Threshold = request.Threshold,
                Type = request.Type,
                Status = AlertStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            mockAlertService.Setup(s => s.CreateAlertAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<decimal>(), It.IsAny<AlertType>()))
                .ReturnsAsync(expectedAlert);

            
            var result = await controller.CreateAlert(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedAlert = Assert.IsType<PriceAlert>(createdResult.Value);
            Assert.Equal(expectedAlert.Id, returnedAlert.Id);
            Assert.Equal(expectedAlert.Symbol, returnedAlert.Symbol);
        }

        [Fact]
        public async Task GetUserAlerts_ValidUserId_ReturnsAlerts()
        {
            // Arrange
            var userId = "testuser";
            var expectedAlerts = new[]
            {
                new PriceAlert { Id = 1, UserId = userId, Symbol = "AAPL", Threshold = 200.00m, Type = AlertType.Above },
                new PriceAlert { Id = 2, UserId = userId, Symbol = "GOOGL", Threshold = 140.00m, Type = AlertType.Below }
            };

            mockAlertService.Setup(s => s.GetUserAlertsAsync(userId))
                .ReturnsAsync(expectedAlerts);

            // Act
            var result = await controller.GetUserAlerts(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAlerts = Assert.IsAssignableFrom<IEnumerable<PriceAlert>>(okResult.Value);
            Assert.Equal(2, returnedAlerts.Count());
        }
    }
}

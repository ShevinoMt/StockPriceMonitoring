using Microsoft.Extensions.Logging;
using Moq;
using StockPriceMonitoring.Core.Enumerations;
using StockPriceMonitoring.Core.Models;
using StockPricingMonitoring.Repositories.Core;
using StockPricingMonitoring.Services;
using StockPricingMonitoring.Services.Core.Interfaces;

namespace StockPricingMonitoring.Tests
{

    public class AlertServiceTests
    {
        private readonly Mock<IPriceAlertRepository> mockAlertRepository;
        private readonly Mock<INotificationService> mockNotificationService;
        private readonly Mock<ILogger<AlertService>> mockLogger;
        private readonly AlertService alertService;

        public AlertServiceTests()
        {
            mockAlertRepository = new Mock<IPriceAlertRepository>();
            mockNotificationService = new Mock<INotificationService>();
            mockLogger = new Mock<ILogger<AlertService>>();
            alertService = new AlertService(mockAlertRepository.Object, mockNotificationService.Object, mockLogger.Object);
        }

        /// <summary>
        /// Create alert test
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateAlertAsync_ValidInput_ReturnsAlert()
        {
            // Arrange
            var userId = "testuser";
            var symbol = "AAPL";
            var threshold = 200.00m;
            var type = AlertType.Above;

            var expectedAlert = new PriceAlert
            {
                Id = 1,
                UserId = userId,
                Symbol = symbol,
                Threshold = threshold,
                Type = type,
                Status = AlertStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            mockAlertRepository.Setup(r => r.CreateAlertAsync(It.IsAny<PriceAlert>()))
                .ReturnsAsync(expectedAlert);

            // Act
            var result = await alertService.CreateAlertAsync(userId, symbol, threshold, type);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(symbol, result.Symbol);
            Assert.Equal(threshold, result.Threshold);
            Assert.Equal(type, result.Type);
            Assert.Equal(AlertStatus.Active, result.Status);
        }

        [Fact]
        public async Task CheckAlertsAsync_AboveThresholdMet_TriggersAlert()
        {
            // Arrange
            var stockPrice = new StockPrice
            {
                Symbol = "AAPL",
                Price = 205.00m,
                Timestamp = DateTime.UtcNow
            };

            var alert = new PriceAlert
            {
                Id = 1,
                UserId = "testuser",
                Symbol = "AAPL",
                Threshold = 200.00m,
                Type = AlertType.Above,
                Status = AlertStatus.Active
            };

            mockAlertRepository.Setup(r => r.GetActiveAlertsAsync())
                .ReturnsAsync(new[] { alert });

            await alertService.CheckAlertsAsync(new[] { stockPrice });

            // Assert
            mockNotificationService.Verify(n => n.SendAlertNotificationAsync(It.IsAny<AlertNotification>()), Times.Once);
            mockAlertRepository.Verify(r => r.UpdateAlertAsync(It.Is<PriceAlert>(a => a.Status == AlertStatus.Triggered)), Times.Once);
        }

        [Fact]
        public async Task CheckAlertsAsync_BelowThresholdNotMet_DoesNotTriggerAlert()
        {
            // Arrange
            var stockPrice = new StockPrice
            {
                Symbol = "AAPL",
                Price = 185.00m,
                Timestamp = DateTime.UtcNow
            };

            var alert = new PriceAlert
            {
                Id = 1,
                UserId = "testuser",
                Symbol = "AAPL",
                Threshold = 180.00m,
                Type = AlertType.Below,
                Status = AlertStatus.Active
            };

            mockAlertRepository.Setup(r => r.GetActiveAlertsAsync())
                .ReturnsAsync(new[] { alert });

            await alertService.CheckAlertsAsync(new[] { stockPrice });

            // Assert
            mockNotificationService.Verify(n => n.SendAlertNotificationAsync(It.IsAny<AlertNotification>()), Times.Never);
            mockAlertRepository.Verify(r => r.UpdateAlertAsync(It.IsAny<PriceAlert>()), Times.Never);
        }
    }
}
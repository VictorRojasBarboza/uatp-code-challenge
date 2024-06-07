using Microsoft.EntityFrameworkCore;
using RapidPay.DAL;
using RapidPay.Service.Services.Card;
using RapidPay.Shared.Utils;
using Xunit;

namespace RapidPay.Test.UnitTest
{
    public class CardServiceTests
    {
        [Fact]
        public async Task CreateCardAsync_ShouldReturnSuccessMessage_WhenCardIsValid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ContextDB>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new ContextDB(options);

            // Create a mock or a real instance of FileLogger
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            var fileLogger = new FileLogger(Path.Combine(logDirectory, "logs.txt"));

            var cardService = new CardService(context, fileLogger);
            string validCardNumber = "123456789012345";

            // Act
            string result = await cardService.CreateCardAsync(validCardNumber);

            // Assert
            Assert.Equal("Card created successfully.", result);
        }

        [Fact]
        public async Task CreateCardAsync_ShouldThrowArgumentException_WhenCardNumberIsInvalid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ContextDB>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new ContextDB(options);//Creating a context with InMemory DB

            // Create a mock or a real instance of FileLogger
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            var fileLogger = new FileLogger(Path.Combine(logDirectory, "logs.txt"));

            var cardService = new CardService(context, fileLogger);
            string invalidCardNumber = "1234567890"; // Less than 15 digits

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => cardService.CreateCardAsync(invalidCardNumber));
        }
    }
}

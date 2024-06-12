using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RapidPay.DAL;
using RapidPay.Service.DTO;
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

            // Create a mock or a real instance of Mapper
            var mapper = Substitute.For<IMapper>();

            // Creating a substitute instance for FileLogger
            var logger = Substitute.For<FileLogger>(Path.Combine(Directory.GetCurrentDirectory(), "logs.txt")); 

            var cardService = new CardService(context, logger, mapper);

            var request = new Service.DTO.CreateCardRequest { CardNumber = "123456789012347", Balance = Convert.ToDecimal(1200.90) };
           
            // Act
            CreateCardResponse result = await cardService.CreateCardAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.CardNumber, result.CardNumber);
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
            var mapper = Substitute.For<IMapper>();
            var logger = Substitute.For<FileLogger>(Path.Combine(Directory.GetCurrentDirectory(), "logs.txt"));
            var cardService = new CardService(context, logger, mapper);

            var request = new Service.DTO.CreateCardRequest { CardNumber = "123456789012347", Balance = Convert.ToDecimal(1200.90) };


            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => cardService.CreateCardAsync(request));
        }
    }
}

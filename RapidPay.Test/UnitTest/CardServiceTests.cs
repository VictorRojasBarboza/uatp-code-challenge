using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RapidPay.DAL;
using RapidPay.DAL.Models;
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

            // Mock the mapping from Card to CreateCardResponse
            mapper.Map<DAL.Models.Card>(Arg.Any<Card>()).Returns(args =>
            {
                var card = args.Arg<Card>();
                return new DAL.Models.Card { CardNumber = card.CardNumber, Balance = card.Balance };
            });

            mapper.Map<Service.DTO.CreateCardResponse>(Arg.Any<DAL.Models.Card>()).Returns(args =>
            {
                var cardDto = args.Arg<DAL.Models.Card>();
                return new Service.DTO.CreateCardResponse { CardNumber = cardDto.CardNumber, Balance = cardDto.Balance };
            });

            // Creating a substitute instance for FileLogger and Cache
            var logger = Substitute.For<FileLogger>(Path.Combine(Directory.GetCurrentDirectory(), "logs.txt"));
            var cache = new MemoryCache(new MemoryCacheOptions());

            var cardService = new CardService(context, logger, mapper,cache);

            var request = new Service.DTO.CreateCardRequest { CardNumber = "123456789012347", Balance = Convert.ToDecimal(1200.90) };
           
            // Act
            var result = await cardService.CreateCardAsync(request);

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

            // Create a mock or a real instance of Mapper
            var mapper = Substitute.For<IMapper>();

            // Mock the mapping from Card to CreateCardResponse
            mapper.Map<DAL.Models.Card>(Arg.Any<Card>()).Returns(args =>
            {
                var card = args.Arg<Card>();
                return new DAL.Models.Card { CardNumber = card.CardNumber, Balance = card.Balance };
            });

            mapper.Map<Service.DTO.CreateCardResponse>(Arg.Any<DAL.Models.Card>()).Returns(args =>
            {
                var cardDto = args.Arg<DAL.Models.Card>();
                return new Service.DTO.CreateCardResponse { CardNumber = cardDto.CardNumber, Balance = cardDto.Balance };
            });

            // Create a mock or a real instance of FileLogger
            var logger = Substitute.For<FileLogger>(Path.Combine(Directory.GetCurrentDirectory(), "logs.txt"));
            var cache = new MemoryCache(new MemoryCacheOptions());

            var cardService = new CardService(context, logger, mapper, cache);

            var request = new Service.DTO.CreateCardRequest { CardNumber = "123", Balance = Convert.ToDecimal(1200.90) };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => cardService.CreateCardAsync(request));
        }
    }
}

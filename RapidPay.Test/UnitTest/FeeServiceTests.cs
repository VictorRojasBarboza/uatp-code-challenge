using Microsoft.Extensions.Caching.Memory;
using RapidPay.Service.Helper;
using RapidPay.Shared.Utils;
using Xunit;

namespace RapidPay.Test.UnitTest
{
    public class FeeServiceTests
    {
        [Fact]
        public void GetCurrentFee_ShouldReturnNewFee_WhenCalled()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var logger = new FileLogger("test.log");

            // Initialize UFE with cache and logger to set the initial fee
            UFE.Initialize(cache, logger);
            var initialFeeService = UFE.Instance;
            
            // Act
            decimal initialFee = initialFeeService.GetCurrentFee();

            // Manually expire the cache by setting a very short expiration time
            cache.Set("CurrentFee", initialFee, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1)
            });

            // Wait for the cache to expire
            Thread.Sleep(10);

            // Initialize UFE again with the same cache, the fee should be recalculated
            var feeService = UFE.Instance;

            decimal newFee = feeService.GetCurrentFee();

            // Assert
            Assert.NotEqual(initialFee, newFee);
        }

        [Fact]
        public void GetCurrentFee_ShouldReturnSameFee_WhenCalledWithinAnHour()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var logger = new FileLogger("test.log");

            // Initialize UFE with cache and logger
            UFE.Initialize(cache, logger);
            var feeService = UFE.Instance;

            // Act
            decimal initialFee = feeService.GetCurrentFee();
            decimal feeWithinSameHour = feeService.GetCurrentFee();

            // Assert
            Assert.Equal(initialFee, feeWithinSameHour);
        }
    }
}

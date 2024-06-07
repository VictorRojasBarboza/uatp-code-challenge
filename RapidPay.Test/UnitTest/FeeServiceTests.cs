using Microsoft.Extensions.Caching.Memory;
using RapidPay.Service.Helper;
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

            // Initialize UFE with default constructor to set the initial fee
            var initialFeeService = new UFE(null, true);

            // Act
            decimal initialFee = initialFeeService.GetCurrentFee();

            // Manually expire the cache by setting a very short expiration time
            cache.Set("CurrentFee", initialFee, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1)
            });

            // Wait for the cache to expire
            System.Threading.Thread.Sleep(10);

            // Initialize UFE with cache and without initializing the fee
            var feeService = new UFE(cache, true);

            decimal newFee = feeService.GetCurrentFee();

            // Assert
            Assert.NotEqual(initialFee, newFee);
        }

        [Fact]
        public void GetCurrentFee_ShouldReturnSameFee_WhenCalledWithinAnHour()
        {
            // Arrange
            var feeService = UFE.Instance;
            decimal initialFee = feeService.GetCurrentFee();

            // Act
            decimal feeWithinSameHour = feeService.GetCurrentFee();

            // Assert
            Assert.Equal(initialFee, feeWithinSameHour);
        }
    }
}

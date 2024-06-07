using Microsoft.Extensions.Caching.Memory;

namespace RapidPay.Service.Helper
{
    public sealed class UFE
    {
        private static readonly Lazy<UFE> lazy = new Lazy<UFE>(() => new UFE());
        private readonly IMemoryCache _cache;
        private readonly Random _random;
        private const string FeeCacheKey = "CurrentFee";
        private readonly decimal initialFee = 0.5m;
        private UFE(bool initializeFee = true)
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _random = new Random();

            if (initializeFee)
            {
                InitializeFee();
            }
        }

        // Constructor with IMemoryCache parameter
        public UFE(IMemoryCache cache, bool initializeFee) : this(false) // Don't initialize the fee when cache is provided
        {
            if (cache != null) 
            {
                _cache = cache;
            }
        }

        public static UFE Instance => lazy.Value;

        private void InitializeFee()
        {
            // Initialize the fee if it does not exist in the cache
            if (!_cache.TryGetValue(FeeCacheKey, out decimal fee))
            {
                fee = initialFee; // Initial fee
                SetFeeInCache(fee);
            }
        }

        public decimal GetCurrentFee()
        {
            if (!_cache.TryGetValue(FeeCacheKey, out decimal fee))
            {
                // Fee expired or not set, calculate a new fee
                fee = CalculateNewFee();
                SetFeeInCache(fee);
            }
            return fee;
        }
        private void SetFeeInCache(decimal fee)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1)); // Set the fee to expire in 1 hour

            _cache.Set(FeeCacheKey, fee, cacheEntryOptions);
        }
        private decimal CalculateNewFee()
        {
            double randomDecimal = _random.NextDouble() * 2; // Random number between 0 and 2
            decimal newFee = Convert.ToDecimal(randomDecimal);
            return newFee;
        }
    }
}

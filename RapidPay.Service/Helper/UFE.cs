using Microsoft.Extensions.Caching.Memory;
using RapidPay.Shared.Utils;

namespace RapidPay.Service.Helper
{
    /// <summary>
    /// Utility Fee Engine (UFE) for managing fee calculations and caching.
    /// </summary>
    public sealed class UFE
    {
        private static UFE _instance;
        private static readonly object lockObject = new object();
        private readonly IMemoryCache _cache;
        private readonly Random _random;
        private const string FeeCacheKey = "CurrentFee";
        private readonly decimal initialFee = 0.5m;
        private readonly FileLogger _logger;

        /// <summary>
        /// Private constructor to prevent direct instantiation. Initializes the cache, random number generator, and optionally the fee.
        /// </summary>
        /// <param name="cache">Memory cache for storing the fee.</param>
        /// <param name="logger">File logger for logging operations.</param>
        /// <param name="initializeFee">Flag indicating whether to initialize the fee.</param>
        private UFE(IMemoryCache cache, FileLogger logger, bool initializeFee = true)
        {
            _cache = cache ?? new MemoryCache(new MemoryCacheOptions());
            _random = new Random();
            _logger = logger;

            if (initializeFee)
            {
                InitializeFee();
            }
        }

        /// <summary>
        /// Initializes the UFE singleton instance with the specified dependencies.
        /// </summary>
        /// <param name="cache">Memory cache for storing the fee.</param>
        /// <param name="logger">File logger for logging operations.</param>
        public static void Initialize(IMemoryCache cache, FileLogger logger)
        {
            lock (lockObject)
            {
                if (_instance == null)
                {
                    _instance = new UFE(cache, logger);
                }
            }
        }

        /// <summary>
        /// Gets the singleton instance of UFE. Throws an exception if not initialized.
        /// </summary>
        public static UFE Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("UFE is not initialized. Call Initialize method first.");
                }

                return _instance;
            }
        }

        /// <summary>
        /// Initializes the fee in the cache if it does not already exist.
        /// </summary>
        private void InitializeFee()
        {
            // Initialize the fee if it does not exist in the cache
            if (!_cache.TryGetValue(FeeCacheKey, out decimal fee))
            {
                fee = initialFee; // Initial fee
                SetFeeInCache(fee);
                _logger?.LogAsync($"Initialized fee: {fee}");
            }
        }

        /// <summary>
        /// Gets the current fee from the cache. If the fee is expired or not set, calculates and sets a new fee.
        /// </summary>
        /// <returns>The current fee.</returns>
        public decimal GetCurrentFee()
        {
            if (!_cache.TryGetValue(FeeCacheKey, out decimal fee))
            {
                // Fee expired or not set, calculate a new fee
                fee = CalculateNewFee();
                SetFeeInCache(fee);
                _logger?.LogAsync($"Calculated new fee: {fee}");
            }
            return fee;
        }

        /// <summary>
        /// Sets the specified fee in the cache with an expiration of 1 hour.
        /// </summary>
        /// <param name="fee">The fee to be cached.</param>
        private void SetFeeInCache(decimal fee)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1)); // Set the fee to expire in 1 hour
                //.SetAbsoluteExpiration(TimeSpan.FromSeconds(20)); // Set the fee to expire in 20 seconds

            _cache.Set(FeeCacheKey, fee, cacheEntryOptions);
            _logger?.LogAsync($"Set fee in cache: {fee}");
        }

        /// <summary>
        /// Calculates a new fee based on a random decimal between 0 and 2.
        /// </summary>
        /// <returns>The newly calculated fee.</returns>
        private decimal CalculateNewFee()
        {
            double randomDecimal = _random.NextDouble() * 2; // Random number between 0 and 2
            decimal newFee = Convert.ToDecimal(randomDecimal);
            _logger?.LogAsync($"Calculated random decimal for new fee: {randomDecimal}, new fee: {newFee}");
            return newFee;
        }
    }
}

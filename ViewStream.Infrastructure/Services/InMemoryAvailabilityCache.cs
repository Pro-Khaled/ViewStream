using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;

namespace ViewStream.Infrastructure.Services
{
    /// <summary>
    /// In-memory implementation of IAvailabilityCache with 15-minute sliding expiry.
    /// Uses MemoryCache for fast lookups without external dependencies.
    /// </summary>
    public class InMemoryAvailabilityCache : IAvailabilityCache
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<InMemoryAvailabilityCache> _logger;
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(15);

        public InMemoryAvailabilityCache(IMemoryCache cache, ILogger<InMemoryAvailabilityCache> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        private static string BuildKey(long showId, string countryCode) =>
            $"availability:{showId}:{countryCode.ToUpperInvariant()}";

        public Task<ShowAvailability?> GetAsync(long showId, string countryCode)
        {
            var key = BuildKey(showId, countryCode);
            _cache.TryGetValue(key, out ShowAvailability? result);
            if (result != null)
                _logger.LogDebug("Availability cache HIT: {Key}", key);
            return Task.FromResult(result);
        }

        public Task SetAsync(long showId, string countryCode, ShowAvailability availability)
        {
            var key = BuildKey(showId, countryCode);
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiry
            };
            _cache.Set(key, availability, options);
            _logger.LogDebug("Availability cached: {Key} (expires in {Minutes} min)", key, CacheExpiry.TotalMinutes);
            return Task.CompletedTask;
        }

        public Task InvalidateAsync(long showId, string countryCode)
        {
            var key = BuildKey(showId, countryCode);
            _cache.Remove(key);
            _logger.LogInformation("Availability cache invalidated: {Key}", key);
            return Task.CompletedTask;
        }
    }
}

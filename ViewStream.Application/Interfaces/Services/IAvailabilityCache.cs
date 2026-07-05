using ViewStream.Domain.Entities;

namespace ViewStream.Application.Interfaces.Services
{
    /// <summary>
    /// Caches show availability records with automatic expiry to reduce DB lookups.
    /// </summary>
    public interface IAvailabilityCache
    {
        /// <summary>
        /// Gets cached availability for a show in a specific country.
        /// Returns null on cache miss.
        /// </summary>
        Task<ShowAvailability?> GetAsync(long showId, string countryCode);

        /// <summary>
        /// Stores an availability record in cache with 15-minute expiry.
        /// </summary>
        Task SetAsync(long showId, string countryCode, ShowAvailability availability);

        /// <summary>
        /// Invalidates the cached availability when an admin updates a ShowAvailability record.
        /// </summary>
        Task InvalidateAsync(long showId, string countryCode);
    }
}

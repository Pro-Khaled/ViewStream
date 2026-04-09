using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for WatchHistory entity
    /// </summary>
    public interface IWatchHistoryRepository : IGenericRepository<WatchHistory>
    {
        // TODO: Add custom methods specific to WatchHistory here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<WatchHistory>> GetActiveAsync();
    }
}

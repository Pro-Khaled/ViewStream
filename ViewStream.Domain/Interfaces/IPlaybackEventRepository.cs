using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for PlaybackEvent entity
    /// </summary>
    public interface IPlaybackEventRepository : IGenericRepository<PlaybackEvent>
    {
        // TODO: Add custom methods specific to PlaybackEvent here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<PlaybackEvent>> GetActiveAsync();
    }
}

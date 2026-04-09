using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for AudioTrack entity
    /// </summary>
    public interface IAudioTrackRepository : IGenericRepository<AudioTrack>
    {
        // TODO: Add custom methods specific to AudioTrack here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<AudioTrack>> GetActiveAsync();
    }
}

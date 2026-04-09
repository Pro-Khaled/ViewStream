using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for WatchPartyParticipant entity
    /// </summary>
    public interface IWatchPartyParticipantRepository : IGenericRepository<WatchPartyParticipant>
    {
        // TODO: Add custom methods specific to WatchPartyParticipant here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<WatchPartyParticipant>> GetActiveAsync();
    }
}

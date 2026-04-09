using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for WatchParty entity
    /// </summary>
    public interface IWatchPartyRepository : IGenericRepository<WatchParty>
    {
        // TODO: Add custom methods specific to WatchParty here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<WatchParty>> GetActiveAsync();
    }
}

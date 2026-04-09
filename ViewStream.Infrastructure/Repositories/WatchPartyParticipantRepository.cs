using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for WatchPartyParticipant entity
    /// </summary>
    public class WatchPartyParticipantRepository : GenericRepository<WatchPartyParticipant>, IWatchPartyParticipantRepository
    {
        public WatchPartyParticipantRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to WatchPartyParticipant here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<WatchPartyParticipant>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

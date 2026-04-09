using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for WatchParty entity
    /// </summary>
    public class WatchPartyRepository : GenericRepository<WatchParty>, IWatchPartyRepository
    {
        public WatchPartyRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to WatchParty here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<WatchParty>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

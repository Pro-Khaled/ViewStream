using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for WatchHistory entity
    /// </summary>
    public class WatchHistoryRepository : GenericRepository<WatchHistory>, IWatchHistoryRepository
    {
        public WatchHistoryRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to WatchHistory here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<WatchHistory>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

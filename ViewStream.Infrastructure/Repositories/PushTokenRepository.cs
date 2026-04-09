using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for PushToken entity
    /// </summary>
    public class PushTokenRepository : GenericRepository<PushToken>, IPushTokenRepository
    {
        public PushTokenRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to PushToken here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<PushToken>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

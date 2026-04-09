using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for ShowAward entity
    /// </summary>
    public class ShowAwardRepository : GenericRepository<ShowAward>, IShowAwardRepository
    {
        public ShowAwardRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to ShowAward here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<ShowAward>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

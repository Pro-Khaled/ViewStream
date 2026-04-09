using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for SharedList entity
    /// </summary>
    public class SharedListRepository : GenericRepository<SharedList>, ISharedListRepository
    {
        public SharedListRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to SharedList here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<SharedList>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

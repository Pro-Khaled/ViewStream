using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for SharedListItem entity
    /// </summary>
    public class SharedListItemRepository : GenericRepository<SharedListItem>, ISharedListItemRepository
    {
        public SharedListItemRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to SharedListItem here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<SharedListItem>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

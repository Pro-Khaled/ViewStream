using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for SearchLog entity
    /// </summary>
    public class SearchLogRepository : GenericRepository<SearchLog>, ISearchLogRepository
    {
        public SearchLogRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to SearchLog here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<SearchLog>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for ContentTag entity
    /// </summary>
    public class ContentTagRepository : GenericRepository<ContentTag>, IContentTagRepository
    {
        public ContentTagRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to ContentTag here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<ContentTag>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

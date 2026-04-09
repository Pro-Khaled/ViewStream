using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for ContentReport entity
    /// </summary>
    public class ContentReportRepository : GenericRepository<ContentReport>, IContentReportRepository
    {
        public ContentReportRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to ContentReport here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<ContentReport>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

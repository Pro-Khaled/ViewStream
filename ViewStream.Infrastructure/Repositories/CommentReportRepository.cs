using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for CommentReport entity
    /// </summary>
    public class CommentReportRepository : GenericRepository<CommentReport>, ICommentReportRepository
    {
        public CommentReportRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to CommentReport here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<CommentReport>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

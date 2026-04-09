using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for EpisodeComment entity
    /// </summary>
    public class EpisodeCommentRepository : GenericRepository<EpisodeComment>, IEpisodeCommentRepository
    {
        public EpisodeCommentRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to EpisodeComment here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<EpisodeComment>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

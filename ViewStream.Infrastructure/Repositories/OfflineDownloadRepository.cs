using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for OfflineDownload entity
    /// </summary>
    public class OfflineDownloadRepository : GenericRepository<OfflineDownload>, IOfflineDownloadRepository
    {
        public OfflineDownloadRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to OfflineDownload here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<OfflineDownload>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

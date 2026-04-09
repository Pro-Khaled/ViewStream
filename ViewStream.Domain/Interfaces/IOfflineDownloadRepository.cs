using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for OfflineDownload entity
    /// </summary>
    public interface IOfflineDownloadRepository : IGenericRepository<OfflineDownload>
    {
        // TODO: Add custom methods specific to OfflineDownload here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<OfflineDownload>> GetActiveAsync();
    }
}

using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for ContentReport entity
    /// </summary>
    public interface IContentReportRepository : IGenericRepository<ContentReport>
    {
        // TODO: Add custom methods specific to ContentReport here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<ContentReport>> GetActiveAsync();
    }
}

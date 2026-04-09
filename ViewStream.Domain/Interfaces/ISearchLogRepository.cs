using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for SearchLog entity
    /// </summary>
    public interface ISearchLogRepository : IGenericRepository<SearchLog>
    {
        // TODO: Add custom methods specific to SearchLog here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<SearchLog>> GetActiveAsync();
    }
}

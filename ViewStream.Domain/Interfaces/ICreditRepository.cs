using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Credit entity
    /// </summary>
    public interface ICreditRepository : IGenericRepository<Credit>
    {
        // TODO: Add custom methods specific to Credit here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Credit>> GetActiveAsync();
    }
}

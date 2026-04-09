using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Award entity
    /// </summary>
    public interface IAwardRepository : IGenericRepository<Award>
    {
        // TODO: Add custom methods specific to Award here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Award>> GetActiveAsync();
    }
}

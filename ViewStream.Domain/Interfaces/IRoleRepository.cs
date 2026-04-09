using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Role entity
    /// </summary>
    public interface IRoleRepository : IGenericRepository<Role>
    {
        // TODO: Add custom methods specific to Role here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Role>> GetActiveAsync();
    }
}

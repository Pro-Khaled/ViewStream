using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Permission entity
    /// </summary>
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        // TODO: Add custom methods specific to Permission here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Permission>> GetActiveAsync();
    }
}

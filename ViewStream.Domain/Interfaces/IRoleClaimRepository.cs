using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for RoleClaim entity
    /// </summary>
    public interface IRoleClaimRepository : IGenericRepository<RoleClaim>
    {
        // TODO: Add custom methods specific to RoleClaim here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<RoleClaim>> GetActiveAsync();
    }
}

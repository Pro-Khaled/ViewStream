using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for UserRole entity
    /// </summary>
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        // TODO: Add custom methods specific to UserRole here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<UserRole>> GetActiveAsync();
    }
}

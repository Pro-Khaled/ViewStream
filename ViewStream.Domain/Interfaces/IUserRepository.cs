using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for User entity
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        // TODO: Add custom methods specific to User here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<User>> GetActiveAsync();
    }
}

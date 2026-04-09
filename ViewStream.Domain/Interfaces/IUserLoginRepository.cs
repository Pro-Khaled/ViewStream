using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for UserLogin entity
    /// </summary>
    public interface IUserLoginRepository : IGenericRepository<UserLogin>
    {
        // TODO: Add custom methods specific to UserLogin here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<UserLogin>> GetActiveAsync();
    }
}

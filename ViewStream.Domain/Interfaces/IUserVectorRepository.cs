using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for UserVector entity
    /// </summary>
    public interface IUserVectorRepository : IGenericRepository<UserVector>
    {
        // TODO: Add custom methods specific to UserVector here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<UserVector>> GetActiveAsync();
    }
}

using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for LoginSession entity
    /// </summary>
    public interface ILoginSessionRepository : IGenericRepository<LoginSession>
    {
        // TODO: Add custom methods specific to LoginSession here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<LoginSession>> GetActiveAsync();
    }
}

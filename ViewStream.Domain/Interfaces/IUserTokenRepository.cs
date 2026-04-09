using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for UserToken entity
    /// </summary>
    public interface IUserTokenRepository : IGenericRepository<UserToken>
    {
        // TODO: Add custom methods specific to UserToken here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<UserToken>> GetActiveAsync();
    }
}

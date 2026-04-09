using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for UserClaim entity
    /// </summary>
    public interface IUserClaimRepository : IGenericRepository<UserClaim>
    {
        // TODO: Add custom methods specific to UserClaim here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<UserClaim>> GetActiveAsync();
    }
}

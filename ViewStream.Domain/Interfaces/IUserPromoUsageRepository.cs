using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for UserPromoUsage entity
    /// </summary>
    public interface IUserPromoUsageRepository : IGenericRepository<UserPromoUsage>
    {
        // TODO: Add custom methods specific to UserPromoUsage here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<UserPromoUsage>> GetActiveAsync();
    }
}

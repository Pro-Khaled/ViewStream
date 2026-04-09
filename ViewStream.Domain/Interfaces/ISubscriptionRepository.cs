using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Subscription entity
    /// </summary>
    public interface ISubscriptionRepository : IGenericRepository<Subscription>
    {
        // TODO: Add custom methods specific to Subscription here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Subscription>> GetActiveAsync();
    }
}

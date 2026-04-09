using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Notification entity
    /// </summary>
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        // TODO: Add custom methods specific to Notification here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Notification>> GetActiveAsync();
    }
}

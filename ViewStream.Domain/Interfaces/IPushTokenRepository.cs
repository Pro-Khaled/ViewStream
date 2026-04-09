using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for PushToken entity
    /// </summary>
    public interface IPushTokenRepository : IGenericRepository<PushToken>
    {
        // TODO: Add custom methods specific to PushToken here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<PushToken>> GetActiveAsync();
    }
}

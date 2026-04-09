using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for ShowAvailability entity
    /// </summary>
    public interface IShowAvailabilityRepository : IGenericRepository<ShowAvailability>
    {
        // TODO: Add custom methods specific to ShowAvailability here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<ShowAvailability>> GetActiveAsync();
    }
}

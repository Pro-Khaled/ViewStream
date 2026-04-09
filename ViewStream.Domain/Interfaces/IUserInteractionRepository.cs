using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for UserInteraction entity
    /// </summary>
    public interface IUserInteractionRepository : IGenericRepository<UserInteraction>
    {
        // TODO: Add custom methods specific to UserInteraction here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<UserInteraction>> GetActiveAsync();
    }
}

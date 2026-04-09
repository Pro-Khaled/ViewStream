using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Profile entity
    /// </summary>
    public interface IProfileRepository : IGenericRepository<Profile>
    {
        // TODO: Add custom methods specific to Profile here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Profile>> GetActiveAsync();
    }
}

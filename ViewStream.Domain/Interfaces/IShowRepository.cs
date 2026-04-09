using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Show entity
    /// </summary>
    public interface IShowRepository : IGenericRepository<Show>
    {
        // TODO: Add custom methods specific to Show here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Show>> GetActiveAsync();
    }
}

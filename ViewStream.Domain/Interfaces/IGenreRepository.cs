using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Genre entity
    /// </summary>
    public interface IGenreRepository : IGenericRepository<Genre>
    {
        // TODO: Add custom methods specific to Genre here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Genre>> GetActiveAsync();
    }
}

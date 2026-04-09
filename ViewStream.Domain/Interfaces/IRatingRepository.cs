using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Rating entity
    /// </summary>
    public interface IRatingRepository : IGenericRepository<Rating>
    {
        // TODO: Add custom methods specific to Rating here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Rating>> GetActiveAsync();
    }
}

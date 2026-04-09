using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for ShowAward entity
    /// </summary>
    public interface IShowAwardRepository : IGenericRepository<ShowAward>
    {
        // TODO: Add custom methods specific to ShowAward here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<ShowAward>> GetActiveAsync();
    }
}

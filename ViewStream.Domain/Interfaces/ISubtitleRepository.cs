using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Subtitle entity
    /// </summary>
    public interface ISubtitleRepository : IGenericRepository<Subtitle>
    {
        // TODO: Add custom methods specific to Subtitle here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Subtitle>> GetActiveAsync();
    }
}

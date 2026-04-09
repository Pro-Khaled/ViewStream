using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Episode entity
    /// </summary>
    public interface IEpisodeRepository : IGenericRepository<Episode>
    {
        // TODO: Add custom methods specific to Episode here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Episode>> GetActiveAsync();
    }
}

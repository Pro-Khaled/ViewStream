using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for EpisodeComment entity
    /// </summary>
    public interface IEpisodeCommentRepository : IGenericRepository<EpisodeComment>
    {
        // TODO: Add custom methods specific to EpisodeComment here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<EpisodeComment>> GetActiveAsync();
    }
}

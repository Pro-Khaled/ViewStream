using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for CommentReport entity
    /// </summary>
    public interface ICommentReportRepository : IGenericRepository<CommentReport>
    {
        // TODO: Add custom methods specific to CommentReport here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<CommentReport>> GetActiveAsync();
    }
}

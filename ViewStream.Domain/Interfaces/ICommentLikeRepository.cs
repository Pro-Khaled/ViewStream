using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for CommentLike entity
    /// </summary>
    public interface ICommentLikeRepository : IGenericRepository<CommentLike>
    {
        // TODO: Add custom methods specific to CommentLike here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<CommentLike>> GetActiveAsync();
    }
}

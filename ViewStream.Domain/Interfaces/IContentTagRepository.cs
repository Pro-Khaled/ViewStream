using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for ContentTag entity
    /// </summary>
    public interface IContentTagRepository : IGenericRepository<ContentTag>
    {
        // TODO: Add custom methods specific to ContentTag here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<ContentTag>> GetActiveAsync();
    }
}

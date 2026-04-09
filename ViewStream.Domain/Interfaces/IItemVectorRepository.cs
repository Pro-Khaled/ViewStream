using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for ItemVector entity
    /// </summary>
    public interface IItemVectorRepository : IGenericRepository<ItemVector>
    {
        // TODO: Add custom methods specific to ItemVector here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<ItemVector>> GetActiveAsync();
    }
}

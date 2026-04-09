using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for SharedListItem entity
    /// </summary>
    public interface ISharedListItemRepository : IGenericRepository<SharedListItem>
    {
        // TODO: Add custom methods specific to SharedListItem here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<SharedListItem>> GetActiveAsync();
    }
}

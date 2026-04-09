using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for SharedList entity
    /// </summary>
    public interface ISharedListRepository : IGenericRepository<SharedList>
    {
        // TODO: Add custom methods specific to SharedList here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<SharedList>> GetActiveAsync();
    }
}

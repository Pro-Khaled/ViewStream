using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Friendship entity
    /// </summary>
    public interface IFriendshipRepository : IGenericRepository<Friendship>
    {
        // TODO: Add custom methods specific to Friendship here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Friendship>> GetActiveAsync();
    }
}

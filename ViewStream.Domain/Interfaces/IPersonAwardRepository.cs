using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for PersonAward entity
    /// </summary>
    public interface IPersonAwardRepository : IGenericRepository<PersonAward>
    {
        // TODO: Add custom methods specific to PersonAward here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<PersonAward>> GetActiveAsync();
    }
}

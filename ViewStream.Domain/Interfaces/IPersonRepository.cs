using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Person entity
    /// </summary>
    public interface IPersonRepository : IGenericRepository<Person>
    {
        // TODO: Add custom methods specific to Person here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Person>> GetActiveAsync();
    }
}

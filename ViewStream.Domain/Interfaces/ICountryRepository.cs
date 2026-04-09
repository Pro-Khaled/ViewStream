using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Country entity
    /// </summary>
    public interface ICountryRepository : IGenericRepository<Country>
    {
        // TODO: Add custom methods specific to Country here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Country>> GetActiveAsync();
    }
}

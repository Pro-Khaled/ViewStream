using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Season entity
    /// </summary>
    public interface ISeasonRepository : IGenericRepository<Season>
    {
        // TODO: Add custom methods specific to Season here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Season>> GetActiveAsync();
    }
}

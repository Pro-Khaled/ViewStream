using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for PersonalizedRow entity
    /// </summary>
    public interface IPersonalizedRowRepository : IGenericRepository<PersonalizedRow>
    {
        // TODO: Add custom methods specific to PersonalizedRow here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<PersonalizedRow>> GetActiveAsync();
    }
}

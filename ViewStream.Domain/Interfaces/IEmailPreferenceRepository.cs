using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for EmailPreference entity
    /// </summary>
    public interface IEmailPreferenceRepository : IGenericRepository<EmailPreference>
    {
        // TODO: Add custom methods specific to EmailPreference here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<EmailPreference>> GetActiveAsync();
    }
}

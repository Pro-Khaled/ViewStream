using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for EmailPreference entity
    /// </summary>
    public class EmailPreferenceRepository : GenericRepository<EmailPreference>, IEmailPreferenceRepository
    {
        public EmailPreferenceRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to EmailPreference here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<EmailPreference>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for LoginSession entity
    /// </summary>
    public class LoginSessionRepository : GenericRepository<LoginSession>, ILoginSessionRepository
    {
        public LoginSessionRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to LoginSession here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<LoginSession>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

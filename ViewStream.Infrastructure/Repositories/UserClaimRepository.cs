using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for UserClaim entity
    /// </summary>
    public class UserClaimRepository : GenericRepository<UserClaim>, IUserClaimRepository
    {
        public UserClaimRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to UserClaim here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<UserClaim>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

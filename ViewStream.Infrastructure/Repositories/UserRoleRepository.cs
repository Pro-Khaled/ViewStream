using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for UserRole entity
    /// </summary>
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to UserRole here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<UserRole>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

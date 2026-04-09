using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for UserInteraction entity
    /// </summary>
    public class UserInteractionRepository : GenericRepository<UserInteraction>, IUserInteractionRepository
    {
        public UserInteractionRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to UserInteraction here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<UserInteraction>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

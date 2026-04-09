using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for UserLibrary entity
    /// </summary>
    public class UserLibraryRepository : GenericRepository<UserLibrary>, IUserLibraryRepository
    {
        public UserLibraryRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to UserLibrary here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<UserLibrary>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

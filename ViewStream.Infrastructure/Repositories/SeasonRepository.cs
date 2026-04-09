using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Season entity
    /// </summary>
    public class SeasonRepository : GenericRepository<Season>, ISeasonRepository
    {
        public SeasonRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to Season here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<Season>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

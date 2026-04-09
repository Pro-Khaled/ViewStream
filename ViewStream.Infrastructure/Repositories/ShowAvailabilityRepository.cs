using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for ShowAvailability entity
    /// </summary>
    public class ShowAvailabilityRepository : GenericRepository<ShowAvailability>, IShowAvailabilityRepository
    {
        public ShowAvailabilityRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to ShowAvailability here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<ShowAvailability>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

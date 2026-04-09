using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for PersonAward entity
    /// </summary>
    public class PersonAwardRepository : GenericRepository<PersonAward>, IPersonAwardRepository
    {
        public PersonAwardRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to PersonAward here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<PersonAward>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

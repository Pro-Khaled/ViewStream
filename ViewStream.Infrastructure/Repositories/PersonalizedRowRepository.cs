using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for PersonalizedRow entity
    /// </summary>
    public class PersonalizedRowRepository : GenericRepository<PersonalizedRow>, IPersonalizedRowRepository
    {
        public PersonalizedRowRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to PersonalizedRow here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<PersonalizedRow>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

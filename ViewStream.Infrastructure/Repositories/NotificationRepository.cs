using Microsoft.EntityFrameworkCore;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Notification entity
    /// </summary>
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ViewStreamDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Notification>> GetFailedPendingAsync(int maxRetryCount = 5)
        {
            return await _dbSet
                .Where(n => n.Status == "Failed" && n.RetryCount < maxRetryCount)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();
        }

        // TODO: Implement custom methods specific to Notification here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<Notification>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Jobs
{
    /// <summary>
    /// Daily Hangfire job that purges watch history records older than 6 months.
    /// </summary>
    public class PurgeOldWatchHistoryJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PurgeOldWatchHistoryJob> _logger;

        public PurgeOldWatchHistoryJob(IServiceScopeFactory scopeFactory, ILogger<PurgeOldWatchHistoryJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Purge old watch history job started.");
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var cutoff = DateTime.UtcNow.AddMonths(-6);
            var oldRecords = await unitOfWork.WatchHistories.FindAsync(
                wh => wh.WatchedAt < cutoff);

            if (oldRecords.Any())
            {
                unitOfWork.WatchHistories.DeleteRange(oldRecords);
                await unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Purged {Count} watch history records older than 6 months.", oldRecords.Count());
            }

            _logger.LogInformation("Purge old watch history job completed.");
        }
    }
}

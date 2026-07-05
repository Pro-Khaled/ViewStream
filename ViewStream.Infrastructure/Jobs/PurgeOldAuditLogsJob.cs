using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Jobs
{
    /// <summary>
    /// Daily Hangfire job that purges audit log records older than 90 days.
    /// </summary>
    public class PurgeOldAuditLogsJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PurgeOldAuditLogsJob> _logger;

        public PurgeOldAuditLogsJob(IServiceScopeFactory scopeFactory, ILogger<PurgeOldAuditLogsJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Purge old audit logs job started.");
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var cutoff = DateTime.UtcNow.AddDays(-90);
            var oldLogs = await unitOfWork.AuditLogs.FindAsync(
                al => al.Timestamp < cutoff);

            if (oldLogs.Any())
            {
                unitOfWork.AuditLogs.DeleteRange(oldLogs);
                await unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Purged {Count} audit log records older than 90 days.", oldLogs.Count());
            }

            _logger.LogInformation("Purge old audit logs job completed.");
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Jobs
{
    /// <summary>
    /// Daily Hangfire job that auto-approves pending data deletion requests older than 30 days.
    /// Per GDPR, organizations must process deletion requests within 30 days.
    /// </summary>
    public class AutoApproveDeletionRequestsJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AutoApproveDeletionRequestsJob> _logger;

        public AutoApproveDeletionRequestsJob(IServiceScopeFactory scopeFactory, ILogger<AutoApproveDeletionRequestsJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Auto-approve deletion requests job started.");
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var cutoff = DateTime.UtcNow.AddDays(-30);
            var pendingRequests = await unitOfWork.DataDeletionRequests.FindAsync(
                r => r.Status == "pending" && r.RequestedAt <= cutoff);

            foreach (var request in pendingRequests)
            {
                request.Status = "approved";
                request.ReviewedAt = DateTime.UtcNow;
                unitOfWork.DataDeletionRequests.Update(request);
                _logger.LogInformation("Auto-approved deletion request {RequestId} (requested {Date})",
                    request.Id, request.RequestedAt);
            }

            if (pendingRequests.Any())
                await unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Auto-approve job completed. {Count} requests approved.", pendingRequests.Count());
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Jobs
{
    /// <summary>
    /// Hourly Hangfire job that closes watch parties inactive for more than 2 hours.
    /// Updates status to 'ended' and removes Redis state.
    /// </summary>
    public class ExpireStaleWatchPartiesJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExpireStaleWatchPartiesJob> _logger;

        public ExpireStaleWatchPartiesJob(IServiceScopeFactory scopeFactory, ILogger<ExpireStaleWatchPartiesJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Expire stale watch parties job started.");
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var cutoff = DateTime.UtcNow.AddHours(-2);
            var staleParties = await unitOfWork.WatchParties.FindAsync(
                wp => wp.Status == "active" && wp.StartedAt < cutoff);

            foreach (var party in staleParties)
            {
                party.Status = "ended";
                party.EndedAt = DateTime.UtcNow;
                unitOfWork.WatchParties.Update(party);
                _logger.LogInformation("Watch party {PartyCode} auto-expired.", party.PartyCode);
            }

            if (staleParties.Any())
                await unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Expire stale watch parties job completed. {Count} parties expired.",
                staleParties.Count());
        }
    }
}

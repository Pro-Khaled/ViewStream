using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Services
{
    /// <summary>
    /// Buffers watch progress updates in-memory and flushes to DB every 5 seconds.
    /// Reduces database write pressure from frequent progress heartbeats.
    /// Registered as a singleton hosted service.
    /// </summary>
    public class InMemoryProgressBufferService : IProgressBufferService, IHostedService, IDisposable
    {
        private readonly ConcurrentDictionary<(long ProfileId, long EpisodeId), int> _buffer = new();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<InMemoryProgressBufferService> _logger;
        private Timer? _flushTimer;
        private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(5);

        public InMemoryProgressBufferService(
            IServiceScopeFactory scopeFactory,
            ILogger<InMemoryProgressBufferService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public Task BufferProgressAsync(long profileId, long episodeId, int progressSeconds)
        {
            var key = (profileId, episodeId);
            _buffer.AddOrUpdate(key, progressSeconds, (_, existing) => Math.Max(existing, progressSeconds));
            return Task.CompletedTask;
        }

        public async Task FlushAsync()
        {
            if (_buffer.IsEmpty) return;

            // Snapshot and clear
            var snapshot = new Dictionary<(long, long), int>();
            foreach (var key in _buffer.Keys.ToList())
            {
                if (_buffer.TryRemove(key, out var progress))
                    snapshot[key] = progress;
            }

            if (snapshot.Count == 0) return;

            _logger.LogDebug("Flushing {Count} buffered progress updates to DB.", snapshot.Count);

            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            foreach (var ((profileId, episodeId), progress) in snapshot)
            {
                try
                {
                    var histories = await unitOfWork.WatchHistories.FindAsync(
                        wh => wh.ProfileId == profileId && wh.EpisodeId == episodeId);
                    var history = histories.FirstOrDefault();

                    if (history != null && progress > (history.ProgressSeconds ?? 0))
                    {
                        history.ProgressSeconds = progress;
                        history.WatchedAt = DateTime.UtcNow;
                        unitOfWork.WatchHistories.Update(history);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to flush progress for Profile {ProfileId}, Episode {EpisodeId}",
                        profileId, episodeId);
                }
            }

            await unitOfWork.SaveChangesAsync();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Progress buffer service started. Flush interval: {Interval}s", FlushInterval.TotalSeconds);
            _flushTimer = new Timer(async _ => await FlushAsync(), null, FlushInterval, FlushInterval);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Progress buffer service stopping. Flushing remaining...");
            _flushTimer?.Change(Timeout.Infinite, 0);
            await FlushAsync();
        }

        public void Dispose()
        {
            _flushTimer?.Dispose();
        }
    }
}

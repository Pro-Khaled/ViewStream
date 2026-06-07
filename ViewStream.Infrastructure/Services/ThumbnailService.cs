using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Infrastructure.Services;

public class ThumbnailService : IThumbnailService
{
    private readonly ILogger<ThumbnailService> _logger;
    public ThumbnailService(ILogger<ThumbnailService> logger) => _logger = logger;

    public async Task GenerateAsync(long episodeId, string rawVideoPath, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Thumbnail generation for episode {EpisodeId} started", episodeId);
        await Task.CompletedTask; // Phase 1 will add FFmpeg
    }
}

namespace ViewStream.Application.Interfaces.Services;

public interface IThumbnailService
{
    Task GenerateAsync(long episodeId, string rawVideoPath, CancellationToken cancellationToken);
}

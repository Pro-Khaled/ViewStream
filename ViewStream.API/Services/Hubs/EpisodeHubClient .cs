using Microsoft.AspNetCore.SignalR;
using ViewStream.Api.Hubs;
using ViewStream.Application.DTOs;
using ViewStream.Application.Interfaces.Services.Hubs;

namespace ViewStream.API.Services.Hubs
{
    public class EpisodeHubClient : IEpisodeHubClient
    {
        private readonly IHubContext<EpisodeHub> _hubContext;

        public EpisodeHubClient(IHubContext<EpisodeHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendEpisodeVideoUpdatedAsync(EpisodeDto episode, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"episode-{episode.Id}").SendAsync("EpisodeVideoUpdated", episode, cancellationToken);
        }

        public async Task SendEpisodeVideoUploadedAsync(long episodeId, string videoUrl, long uploadedBy, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All.SendAsync("EpisodeVideoUploaded", new { EpisodeId = episodeId, VideoUrl = videoUrl, UploadedBy = uploadedBy, Timestamp = DateTime.UtcNow }, cancellationToken);
        }

        public async Task SendEpisodeThumbnailUpdatedAsync(EpisodeDto episode, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"episode-{episode.Id}").SendAsync("EpisodeThumbnailUpdated", episode, cancellationToken);
        }

        public async Task SendEpisodeThumbnailUploadedAsync(long episodeId, string thumbnailUrl, long uploadedBy, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All.SendAsync("EpisodeThumbnailUploaded", new { EpisodeId = episodeId, ThumbnailUrl = thumbnailUrl, UploadedBy = uploadedBy, Timestamp = DateTime.UtcNow }, cancellationToken);
        }

        public async Task SendAudioTrackFileUpdatedAsync(AudioTrackDto audioTrack, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"episode-{audioTrack.EpisodeId}")
                .SendAsync("AudioTrackFileUpdated", audioTrack, cancellationToken);
        }

        public async Task SendSubtitleFileUpdatedAsync(SubtitleDto subtitle, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"episode-{subtitle.EpisodeId}")
                .SendAsync("SubtitleFileUpdated", subtitle, cancellationToken);
        }
    }
}

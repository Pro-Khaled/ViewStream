using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Interfaces.Services.Hubs
{
    public interface IEpisodeHubClient
    {
        Task SendEpisodeVideoUpdatedAsync(EpisodeDto episode, CancellationToken cancellationToken = default);
        Task SendEpisodeVideoUploadedAsync(long episodeId, string videoUrl, long uploadedBy, CancellationToken cancellationToken = default);
        Task SendEpisodeThumbnailUpdatedAsync(EpisodeDto episode, CancellationToken cancellationToken = default);
        Task SendEpisodeThumbnailUploadedAsync(long episodeId, string thumbnailUrl, long uploadedBy, CancellationToken cancellationToken = default);

        // New methods for audio and subtitles
        Task SendAudioTrackFileUpdatedAsync(AudioTrackDto audioTrack, CancellationToken cancellationToken = default);
        Task SendSubtitleFileUpdatedAsync(SubtitleDto subtitle, CancellationToken cancellationToken = default);
    }
}

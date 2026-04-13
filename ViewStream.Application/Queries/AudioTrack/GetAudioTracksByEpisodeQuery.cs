using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.AudioTrack
{
    public record GetAudioTracksByEpisodeQuery(long EpisodeId, bool IncludeDeleted = false) : IRequest<List<AudioTrackListItemDto>>;

}

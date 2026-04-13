using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.AudioTrack
{
    public record GetAudioTrackByIdQuery(long Id) : IRequest<AudioTrackDto?>;

}

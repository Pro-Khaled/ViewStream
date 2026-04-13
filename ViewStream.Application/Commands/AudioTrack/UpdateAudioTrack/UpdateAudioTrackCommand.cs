using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.AudioTrack.UpdateAudioTrack
{
    public record UpdateAudioTrackCommand(long Id, UpdateAudioTrackDto Dto) : IRequest<bool>;

}

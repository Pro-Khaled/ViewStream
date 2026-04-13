using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.AudioTrack.CreateAudioTrack
{
    public record CreateAudioTrackCommand(CreateAudioTrackDto Dto) : IRequest<long>;

}

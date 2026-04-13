using MediatR;

namespace ViewStream.Application.Commands.AudioTrack.DeleteAudioTrack
{
    public record DeleteAudioTrackCommand(long Id) : IRequest<bool>;       // Soft delete

}

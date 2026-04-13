using MediatR;


namespace ViewStream.Application.Commands.AudioTrack.RestoreAudioTrack
{
    public record RestoreAudioTrackCommand(long Id) : IRequest<bool>;

}

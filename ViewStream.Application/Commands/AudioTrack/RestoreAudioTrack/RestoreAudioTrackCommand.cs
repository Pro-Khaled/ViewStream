using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.AudioTrack.RestoreAudioTrack
{
    public record RestoreAudioTrackCommand(long Id, long RestoredByUserId)
    : IRequest<bool>, IHasUserId
    {
        public long? UserId => RestoredByUserId;
    }
}


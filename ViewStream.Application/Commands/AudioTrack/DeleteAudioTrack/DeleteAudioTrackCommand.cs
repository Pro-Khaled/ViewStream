using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.AudioTrack.DeleteAudioTrack
{
    public record DeleteAudioTrackCommand(long Id, long DeletedByUserId)
    : IRequest<bool>, IHasUserId
    {
        public long? UserId => DeletedByUserId;
    }
}


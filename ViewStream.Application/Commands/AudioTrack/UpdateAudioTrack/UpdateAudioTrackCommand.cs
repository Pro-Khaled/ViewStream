using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.AudioTrack.UpdateAudioTrack
{
    public record UpdateAudioTrackCommand(long Id, UpdateAudioTrackDto Dto, long UpdatedByUserId)
    : IRequest<bool>, IHasUserId
    {
        public long? UserId => UpdatedByUserId;
    }
}
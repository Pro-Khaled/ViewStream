using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.AudioTrack.CreateAudioTrack
{
    public record CreateAudioTrackCommand(CreateAudioTrackDto Dto, long CreatedByUserId)
    : IRequest<long>, IHasUserId
    {
        public long? UserId => CreatedByUserId;
    }
}


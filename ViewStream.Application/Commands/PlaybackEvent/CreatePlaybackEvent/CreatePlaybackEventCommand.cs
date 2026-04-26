using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PlaybackEvent.CreatePlaybackEvent
{
    public record CreatePlaybackEventCommand(long ProfileId, CreatePlaybackEventDto Dto, long ActorUserId)
        : IRequest<PlaybackEventDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

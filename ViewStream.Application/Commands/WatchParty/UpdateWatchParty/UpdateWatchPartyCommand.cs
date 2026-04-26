using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.WatchParty.UpdateWatchParty
{
    public record UpdateWatchPartyCommand(long Id, long ProfileId, UpdateWatchPartyDto Dto, long ActorUserId)
        : IRequest<WatchPartyDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

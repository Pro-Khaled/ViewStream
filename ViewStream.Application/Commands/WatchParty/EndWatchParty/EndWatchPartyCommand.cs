using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.WatchParty.DeleteWatchParty
{
    public record EndWatchPartyCommand(long Id, long ProfileId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

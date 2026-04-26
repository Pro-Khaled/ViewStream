using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.WatchPartyParticipant.LeaveWatchParty
{
    public record LeaveWatchPartyCommand(long PartyId, long ProfileId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

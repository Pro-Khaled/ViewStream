using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.WatchPartyParticipant.JoinWatchParty
{
    public record JoinWatchPartyCommand(long PartyId, long ProfileId, long ActorUserId)
        : IRequest<WatchPartyParticipantDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.WatchPartyParticipant.RemoveWatchPartyParticipantAdmin
{
    public record RemoveWatchPartyParticipantAdminCommand(long PartyId, long ProfileId, long AdminUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => AdminUserId;
    }
}

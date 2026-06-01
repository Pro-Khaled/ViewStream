using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.WatchParty.DeleteWatchPartyAdmin
{
    public record DeleteWatchPartyAdminCommand(long Id, long AdminUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => AdminUserId;
    }
}

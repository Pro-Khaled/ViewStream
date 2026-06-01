using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.SharedListItem.DeleteSharedListItemAdmin
{
    public record DeleteSharedListItemAdminCommand(long ShowId, long AdminUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => AdminUserId;
    }
}

using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Notification.DeleteNotificationAdmin
{
    public record DeleteNotificationAdminCommand(long Id, long AdminUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => AdminUserId;
    }
}

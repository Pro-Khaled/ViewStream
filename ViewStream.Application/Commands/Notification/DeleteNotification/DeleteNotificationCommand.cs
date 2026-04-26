using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Notification.DeleteNotification
{
    public record DeleteNotificationCommand(long Id, long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

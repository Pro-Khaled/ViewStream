using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Notification.MarkAllNotificationsAsRead
{
    public record MarkAllNotificationsAsReadCommand(long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

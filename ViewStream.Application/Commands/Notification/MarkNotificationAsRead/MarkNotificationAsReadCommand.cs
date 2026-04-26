using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Notification.MarkNotificationAsRead
{
    public record MarkNotificationAsReadCommand(long Id, long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

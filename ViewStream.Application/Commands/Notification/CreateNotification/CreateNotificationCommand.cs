using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Notification.CreateNotification
{
    public record CreateNotificationCommand(CreateNotificationDto Dto, long ActorUserId)
        : IRequest<NotificationDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

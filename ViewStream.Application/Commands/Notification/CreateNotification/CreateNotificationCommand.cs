using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Notification.CreateNotification
{
    public record CreateNotificationCommand(CreateNotificationDto Dto) : IRequest<NotificationDto>;

}

using MediatR;

namespace ViewStream.Application.Commands.Notification.DeleteNotification
{
    public record DeleteNotificationCommand(long Id, long UserId) : IRequest<bool>;

}

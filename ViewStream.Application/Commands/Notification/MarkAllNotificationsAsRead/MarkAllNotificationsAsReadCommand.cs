using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.Notification.MarkAllNotificationsAsRead
{
    public record MarkAllNotificationsAsReadCommand(long UserId) : IRequest<bool>;

}

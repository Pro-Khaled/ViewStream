using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.Notification.MarkNotificationAsRead
{
    public record MarkNotificationAsReadCommand(long Id, long UserId) : IRequest<bool>;

}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Notification
{
    public record GetUserNotificationsQuery(long UserId, bool UnreadOnly = false, int Limit = 50) : IRequest<List<NotificationDto>>;

}

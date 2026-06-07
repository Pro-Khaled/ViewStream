using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Interfaces.Services
{
    public interface IInAppNotificationSender
    {
        Task SendToUserAsync(long userId, object notificationData, CancellationToken cancellationToken = default);
    }
}

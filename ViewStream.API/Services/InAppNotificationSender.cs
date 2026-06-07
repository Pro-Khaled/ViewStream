using Microsoft.AspNetCore.SignalR;
using ViewStream.API.Hubs;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.API.Services
{
    public class InAppNotificationSender : IInAppNotificationSender
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public InAppNotificationSender(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(long userId, object notificationData, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", notificationData, cancellationToken);
        }
    }
}

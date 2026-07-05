using Microsoft.AspNetCore.SignalR;
using ViewStream.API.Hubs;
using ViewStream.Application.Interfaces.Services.Hubs;

namespace ViewStream.API.Services.Hubs
{
    /// <summary>
    /// Implementation of the IAdminNotificationHubClient interface using SignalR's AdminNotificationHub.
    /// </summary>
    public class AdminNotificationHubClient : IAdminNotificationHubClient
    {
        private readonly IHubContext<AdminNotificationHub> _hubContext;

        public AdminNotificationHubClient(IHubContext<AdminNotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendModerationAlertAsync(string entityType, long entityId, int reportCount, string? details = null, CancellationToken cancellationToken = default)
        {
            // Call the static helper on the hub class
            await AdminNotificationHub.SendModerationAlert(_hubContext, entityType, entityId, reportCount, details);
        }

        public async Task SendNotificationAsync(string title, string message, string severity = "info", CancellationToken cancellationToken = default)
        {
            // Call the static helper on the hub class
            await AdminNotificationHub.SendNotification(_hubContext, title, message, severity);
        }
    }
}

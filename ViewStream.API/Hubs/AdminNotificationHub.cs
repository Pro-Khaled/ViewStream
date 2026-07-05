using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ViewStream.API.Hubs
{
    /// <summary>
    /// SignalR hub for admin dashboard real-time notifications.
    /// Used to push moderation alerts (auto-hidden content, report thresholds).
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminNotificationHub : Hub
    {
        /// <summary>
        /// Sends a moderation alert to all connected admin clients.
        /// </summary>
        public static async Task SendModerationAlert(
            IHubContext<AdminNotificationHub> hubContext,
            string entityType, long entityId, int reportCount, string? details = null)
        {
            await hubContext.Clients.All.SendAsync("ModerationAlert", new
            {
                EntityType = entityType,
                EntityId = entityId,
                ReportCount = reportCount,
                Details = details,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Sends a general admin notification.
        /// </summary>
        public static async Task SendNotification(
            IHubContext<AdminNotificationHub> hubContext,
            string title, string message, string severity = "info")
        {
            await hubContext.Clients.All.SendAsync("AdminNotification", new
            {
                Title = title,
                Message = message,
                Severity = severity,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}

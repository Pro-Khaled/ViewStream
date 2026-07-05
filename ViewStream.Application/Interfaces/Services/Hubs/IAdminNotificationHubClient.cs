using System.Threading;
using System.Threading.Tasks;

namespace ViewStream.Application.Interfaces.Services.Hubs
{
    /// <summary>
    /// Abstraction for broadcasting notifications to connected admin SignalR clients.
    /// </summary>
    public interface IAdminNotificationHubClient
    {
        /// <summary>Sends a moderation alert about reported/hidden content.</summary>
        Task SendModerationAlertAsync(string entityType, long entityId, int reportCount, string? details = null, CancellationToken cancellationToken = default);

        /// <summary>Sends a general admin notification.</summary>
        Task SendNotificationAsync(string title, string message, string severity = "info", CancellationToken cancellationToken = default);
    }
}

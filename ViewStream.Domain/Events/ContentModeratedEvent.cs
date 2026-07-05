using MediatR;

namespace ViewStream.Domain.Events
{
    /// <summary>
    /// Raised when a comment or content (episode) is auto-hidden due to report threshold or moderation.
    /// </summary>
    public record ContentModeratedEvent(string EntityType, long EntityId, int ReportCount, string? Details = null) : INotification;
}

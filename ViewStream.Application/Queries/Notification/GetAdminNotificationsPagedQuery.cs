using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Notification
{
    public record GetAdminNotificationsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminNotificationListItemDto>>
    {
        public long? UserId { get; init; }
        public bool? IsRead { get; init; }

        public GetAdminNotificationsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? userId = null, bool? isRead = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            UserId = userId;
            IsRead = isRead;
        }
    }
}

using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.LoginSession
{
    public record GetAdminLoginSessionsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminLoginSessionListItemDto>>
    {
        public long? UserId { get; init; }
        public bool? IsActive { get; init; }

        public GetAdminLoginSessionsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? userId = null, bool? isActive = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            UserId = userId;
            IsActive = isActive;
        }
    }
}

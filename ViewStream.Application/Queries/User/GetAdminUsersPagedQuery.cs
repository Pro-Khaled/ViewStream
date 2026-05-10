using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.User
{
    public record GetAdminUsersPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminUserListItemDto>>
    {
        public bool? IsActive { get; init; }
        public bool? IsBlocked { get; init; }

        public GetAdminUsersPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, bool? isActive = null, bool? isBlocked = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            IsActive = isActive;
            IsBlocked = isBlocked;
        }
    }
}

using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.UserLibrary
{
    public record GetAdminUserLibrariesPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminUserLibraryListItemDto>>
    {
        public long? ProfileId { get; init; }
        public string? Status { get; init; }

        public GetAdminUserLibrariesPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? profileId = null, string? status = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            ProfileId = profileId;
            Status = status;
        }
    }
}

using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.SharedList
{
    public record GetAdminSharedListsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminSharedListListItemDto>>
    {
        public long? OwnerProfileId { get; init; }
        public bool? IsPublic { get; init; }

        public GetAdminSharedListsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? ownerProfileId = null, bool? isPublic = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            OwnerProfileId = ownerProfileId;
            IsPublic = isPublic;
        }
    }
}

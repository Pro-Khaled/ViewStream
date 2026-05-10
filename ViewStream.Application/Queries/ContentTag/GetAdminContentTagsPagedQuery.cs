using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ContentTag
{
    public record GetAdminContentTagsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminContentTagListItemDto>>
    {
        public string? Category { get; init; }

        public GetAdminContentTagsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, string? category = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            Category = category;
        }
    }
}

using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ContentTag
{
    public record GetContentTagsPagedQuery(
        int Page = 1,
        int PageSize = 20,
        string? SearchTerm = null
    ) : IRequest<PagedResult<ContentTagListItemDto>>;
    public record GetAllContentTagsQuery : IRequest<List<ContentTagListItemDto>>;


}

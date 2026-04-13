using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.UserLibrary
{
    public record GetUserLibraryPagedQuery(
        long ProfileId,
        int Page = 1,
        int PageSize = 20,
        string? Status = null
    ) : IRequest<PagedResult<UserLibraryListItemDto>>;
}

using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Genre
{
    public record GetGenresPagedQuery(
        int Page = 1,
        int PageSize = 20,
        string? SearchTerm = null
    ) : IRequest<PagedResult<GenreListItemDto>>;

    public record GetAllGenresQuery : IRequest<List<GenreListItemDto>>; // For dropdowns
}

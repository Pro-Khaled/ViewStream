using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Person
{
    public record GetAllPersonsQuery : IRequest<List<PersonListItemDto>>;

    public record GetPersonsPagedQuery(int Page = 1, int PageSize = 20, string? SearchTerm = null) : IRequest<PagedResult<PersonListItemDto>>;

}

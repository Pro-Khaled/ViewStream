using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Country
{
    public record GetCountriesPagedQuery(
        int Page = 1,
        int PageSize = 20,
        string? SearchTerm = null,
        string? Continent = null
    ) : IRequest<PagedResult<CountryListItemDto>>;

    public record GetAllCountriesQuery : IRequest<List<CountryListItemDto>>;
}

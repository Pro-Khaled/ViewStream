using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Country
{
    public record GetAdminCountriesPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminCountryListItemDto>>
    {
        public string? Continent { get; init; }

        public GetAdminCountriesPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, string? continent = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            Continent = continent;
        }
    }
}

using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ShowAvailability
{
    public record GetAdminShowAvailabilitiesPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminShowAvailabilityListItemDto>>
    {
        public long? ShowId { get; init; }
        public string? CountryCode { get; init; }

        public GetAdminShowAvailabilitiesPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? showId = null, string? countryCode = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            ShowId = showId;
            CountryCode = countryCode;
        }
    }
}

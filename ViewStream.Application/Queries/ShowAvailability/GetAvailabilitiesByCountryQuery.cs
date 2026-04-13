using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ShowAvailability
{
    public record GetAvailabilitiesByCountryQuery(string CountryCode) : IRequest<List<ShowAvailabilityListItemDto>>;
}

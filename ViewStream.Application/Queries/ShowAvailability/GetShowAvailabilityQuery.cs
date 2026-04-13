using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ShowAvailability
{
    public record GetShowAvailabilityQuery(long ShowId, string CountryCode) : IRequest<ShowAvailabilityDto?>;
}

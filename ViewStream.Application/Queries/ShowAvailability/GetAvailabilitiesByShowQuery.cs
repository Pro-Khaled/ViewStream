using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ShowAvailability
{
    public record GetAvailabilitiesByShowQuery(long ShowId) : IRequest<List<ShowAvailabilityListItemDto>>;
}

using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ShowAvailability.UpdateShowAvailability
{
    public record UpdateShowAvailabilityCommand(long ShowId, string CountryCode, UpdateShowAvailabilityDto Dto) : IRequest<bool>;
}

using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ShowAvailability.CreateShowAvailability
{
    public record CreateShowAvailabilityCommand(CreateShowAvailabilityDto Dto) : IRequest<(long ShowId, string CountryCode)>;
}

using MediatR;

namespace ViewStream.Application.Commands.ShowAvailability.DeleteShowAvailability
{
    public record DeleteShowAvailabilityCommand(long ShowId, string CountryCode) : IRequest<bool>;

}

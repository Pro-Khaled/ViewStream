using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ShowAvailability.CreateShowAvailability
{
    public record CreateShowAvailabilityCommand(CreateShowAvailabilityDto Dto, long ActorUserId)
        : IRequest<(long ShowId, string CountryCode)>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

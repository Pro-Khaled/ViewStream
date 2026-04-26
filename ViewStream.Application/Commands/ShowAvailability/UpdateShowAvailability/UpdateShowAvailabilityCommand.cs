using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ShowAvailability.UpdateShowAvailability
{
    public record UpdateShowAvailabilityCommand(long ShowId, string CountryCode, UpdateShowAvailabilityDto Dto, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

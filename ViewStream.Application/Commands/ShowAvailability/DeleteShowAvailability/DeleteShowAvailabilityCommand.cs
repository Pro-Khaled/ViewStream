using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.ShowAvailability.DeleteShowAvailability
{
    public record DeleteShowAvailabilityCommand(long ShowId, string CountryCode, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

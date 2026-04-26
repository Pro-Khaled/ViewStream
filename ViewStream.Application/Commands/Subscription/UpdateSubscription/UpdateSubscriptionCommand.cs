using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Subscription.UpdateSubscription
{
    public record UpdateSubscriptionCommand(long Id, UpdateSubscriptionDto Dto, long ActorUserId)
        : IRequest<SubscriptionDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

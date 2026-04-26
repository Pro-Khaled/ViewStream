using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Subscription.DeleteSubscription
{
    public record CancelSubscriptionCommand(long Id, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

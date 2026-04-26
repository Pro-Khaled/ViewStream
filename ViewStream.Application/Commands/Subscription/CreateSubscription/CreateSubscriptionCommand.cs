using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Subscription.CreateSubscription
{
    public record CreateSubscriptionCommand(long UserId, CreateSubscriptionDto Dto, long ActorUserId)
        : IRequest<SubscriptionDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

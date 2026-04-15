using MediatR;

namespace ViewStream.Application.Commands.Subscription.DeleteSubscription
{
    public record CancelSubscriptionCommand(long Id) : IRequest<bool>;

}

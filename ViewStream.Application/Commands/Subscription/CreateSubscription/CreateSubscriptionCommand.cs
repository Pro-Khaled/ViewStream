using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Subscription.CreateSubscription
{
    public record CreateSubscriptionCommand(long UserId, CreateSubscriptionDto Dto) : IRequest<SubscriptionDto>;

}

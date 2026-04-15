using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Subscription.UpdateSubscription
{
    public record UpdateSubscriptionCommand(long Id, UpdateSubscriptionDto Dto) : IRequest<SubscriptionDto?>;

}

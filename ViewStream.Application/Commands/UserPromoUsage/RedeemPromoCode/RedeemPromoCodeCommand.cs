using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserPromoUsage.RedeemPromoCode
{
    public record RedeemPromoCodeCommand(long UserId, string Code, string? PlanType, long ActorUserId)
        : IRequest<UserPromoUsageDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

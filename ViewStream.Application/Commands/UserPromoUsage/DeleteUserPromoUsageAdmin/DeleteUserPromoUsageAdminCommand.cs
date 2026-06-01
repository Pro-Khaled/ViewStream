using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.UserPromoUsage.DeleteUserPromoUsageAdmin
{
    public record DeleteUserPromoUsageAdminCommand(long UserId, int PromoCodeId, long AdminUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => AdminUserId;
    }
}

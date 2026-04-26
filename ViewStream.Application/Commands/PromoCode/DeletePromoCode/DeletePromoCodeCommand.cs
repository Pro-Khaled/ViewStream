using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.PromoCode.DeletePromoCode
{
    public record DeletePromoCodeCommand(int Id, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PromoCode.CreatePromoCode
{
    public record CreatePromoCodeCommand(CreatePromoCodeDto Dto, long ActorUserId)
        : IRequest<PromoCodeDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

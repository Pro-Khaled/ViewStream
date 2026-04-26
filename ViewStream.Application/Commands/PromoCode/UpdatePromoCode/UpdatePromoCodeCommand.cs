using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PromoCode.UpdatePromoCode
{
    public record UpdatePromoCodeCommand(int Id, UpdatePromoCodeDto Dto, long ActorUserId)
        : IRequest<PromoCodeDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

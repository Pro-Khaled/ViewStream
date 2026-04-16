using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PromoCode.CreatePromoCode
{
    public record CreatePromoCodeCommand(CreatePromoCodeDto Dto) : IRequest<PromoCodeDto>;

}

using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PromoCode.UpdatePromoCode
{
    public record UpdatePromoCodeCommand(int Id, UpdatePromoCodeDto Dto) : IRequest<PromoCodeDto?>;

}

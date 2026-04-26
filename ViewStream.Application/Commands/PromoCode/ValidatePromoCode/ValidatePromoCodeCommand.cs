using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PromoCode.ValidatePromoCode
{
    public record ValidatePromoCodeCommand(ValidatePromoCodeDto Dto) : IRequest<PromoCodeValidationResultDto>;

}

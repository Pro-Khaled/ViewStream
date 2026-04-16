using MediatR;

namespace ViewStream.Application.Commands.PromoCode.DeletePromoCode
{
    public record DeletePromoCodeCommand(int Id) : IRequest<bool>;

}

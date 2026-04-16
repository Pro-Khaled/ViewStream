using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.PromoCode
{
    public record GetPromoCodeByIdQuery(int Id) : IRequest<PromoCodeDto?>;

}

using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.PaymentMethod
{
    public record GetPaymentMethodByIdQuery(long Id, long UserId) : IRequest<PaymentMethodDto?>;

}

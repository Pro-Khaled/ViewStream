using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PaymentMethod.CreatePaymentMethod
{
    public record AddPaymentMethodCommand(long UserId, CreatePaymentMethodDto Dto) : IRequest<PaymentMethodDto>;

}

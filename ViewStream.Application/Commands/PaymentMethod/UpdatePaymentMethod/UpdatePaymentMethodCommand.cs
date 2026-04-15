using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PaymentMethod.UpdatePaymentMethod
{
    public record UpdatePaymentMethodCommand(long Id, long UserId, UpdatePaymentMethodDto Dto) : IRequest<PaymentMethodDto?>;

}

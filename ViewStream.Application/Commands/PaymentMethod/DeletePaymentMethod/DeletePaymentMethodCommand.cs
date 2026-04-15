using MediatR;
using ViewStream.Application.Common;

namespace ViewStream.Application.Commands.PaymentMethod.DeletePaymentMethod
{
    public record DeletePaymentMethodCommand(long Id, long UserId) : IRequest<bool>;

}

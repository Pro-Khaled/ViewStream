using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.PaymentMethod.DeletePaymentMethodAdmin
{
    public record DeletePaymentMethodAdminCommand(long Id, long AdminUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => AdminUserId;
    }
}

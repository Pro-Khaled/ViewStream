using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.PaymentMethod.DeletePaymentMethod
{
    public record DeletePaymentMethodCommand(long Id, long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

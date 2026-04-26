using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.PaymentMethod.SetDefaultPaymentMethod
{
    public record SetDefaultPaymentMethodCommand(long Id, long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

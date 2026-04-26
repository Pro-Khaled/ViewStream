using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PaymentMethod.UpdatePaymentMethod
{
    public record UpdatePaymentMethodCommand(long Id, long UserId, UpdatePaymentMethodDto Dto, long ActorUserId)
        : IRequest<PaymentMethodDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

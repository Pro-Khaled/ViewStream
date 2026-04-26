using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PaymentMethod.CreatePaymentMethod
{
    public record AddPaymentMethodCommand(long UserId, CreatePaymentMethodDto Dto, long ActorUserId)
        : IRequest<PaymentMethodDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

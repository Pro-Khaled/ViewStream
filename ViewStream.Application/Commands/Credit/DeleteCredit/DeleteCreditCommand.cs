using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Credit.DeleteCredit
{
    public record DeleteCreditCommand(long Id, long UserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}

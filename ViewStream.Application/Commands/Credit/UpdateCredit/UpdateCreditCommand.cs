using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Credit.UpdateCredit
{
    public record UpdateCreditCommand(long Id, UpdateCreditDto Dto, long UserId)
        : IRequest<CreditDto?>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}

using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Credit.CreateCredit
{
    public record CreateCreditCommand(CreateCreditDto Dto, long UserId)
        : IRequest<CreditDto>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}

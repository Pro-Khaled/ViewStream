using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Credit.CreateCredit
{
    public record CreateCreditCommand(CreateCreditDto Dto) : IRequest<CreditDto>;

}

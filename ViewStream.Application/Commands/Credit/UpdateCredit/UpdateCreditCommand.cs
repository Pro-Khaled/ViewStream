using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Credit.UpdateCredit
{
    public record UpdateCreditCommand(long Id, UpdateCreditDto Dto) : IRequest<CreditDto?>;

}

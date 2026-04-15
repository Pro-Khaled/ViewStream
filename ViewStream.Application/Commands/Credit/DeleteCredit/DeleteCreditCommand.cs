using MediatR;

namespace ViewStream.Application.Commands.Credit.DeleteCredit
{
    public record DeleteCreditCommand(long Id) : IRequest<bool>;

}

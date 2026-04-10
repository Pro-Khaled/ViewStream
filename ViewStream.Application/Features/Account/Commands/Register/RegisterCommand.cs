using MediatR;
using ViewStream.Application.DTOs.Account;

namespace ViewStream.Application.Features.Account.Commands.Register
{
    public record RegisterCommand(RegisterDto Dto) : IRequest<RegisterResult>;

    public record RegisterResult(bool Succeeded, IEnumerable<string> Errors, bool RequiresEmailConfirmation = false,bool ConfirmationResent = false);
}

using MediatR;
using ViewStream.Application.DTOs.Account;

namespace ViewStream.Application.Commands.Account.ResetPassword
{
    public record ResetPasswordCommand(ResetPasswordDto Dto) : IRequest<ResetPasswordResult>;

    public record ResetPasswordResult(bool Succeeded, IEnumerable<string> Errors);
}

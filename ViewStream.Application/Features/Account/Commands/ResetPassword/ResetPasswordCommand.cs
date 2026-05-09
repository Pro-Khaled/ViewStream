using MediatR;
using ViewStream.Application.DTOs.Account;

namespace ViewStream.Application.Features.Account.Commands.ResetPassword
{
    public record ResetPasswordCommand(ResetPasswordDto Dto) : IRequest<ResetPasswordResult>;

    public record ResetPasswordResult(bool Succeeded, IEnumerable<string> Errors);
}

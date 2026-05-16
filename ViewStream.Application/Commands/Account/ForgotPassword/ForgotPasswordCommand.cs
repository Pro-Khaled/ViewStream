using MediatR;

namespace ViewStream.Application.Commands.Account.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<bool>;
}

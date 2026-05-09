using MediatR;

namespace ViewStream.Application.Features.Account.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<bool>;
}

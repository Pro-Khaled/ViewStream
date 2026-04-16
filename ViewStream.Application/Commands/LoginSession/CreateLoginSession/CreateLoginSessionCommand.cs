using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.LoginSession.CreateLoginSession
{
    public record CreateLoginSessionCommand(
        long UserId,
        long? DeviceId,
        string SessionToken,
        string? IpAddress,
        string? UserAgent,
        DateTime ExpiresAt) : IRequest<LoginSessionDto>;
}

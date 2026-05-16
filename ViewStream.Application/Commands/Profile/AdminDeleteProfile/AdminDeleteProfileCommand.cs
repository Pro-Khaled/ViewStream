using MediatR;

namespace ViewStream.Application.Commands.Profile.AdminDeleteProfile
{
    public record AdminDeleteProfileCommand(long ProfileId, long AdminUserId) : IRequest<bool>;
}

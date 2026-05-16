using MediatR;

namespace ViewStream.Application.Commands.Profile.RestoreProfile
{
    public record RestoreProfileCommand(long ProfileId, long AdminUserId) : IRequest<bool>;
}

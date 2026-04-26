using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Profile.CreateProfile
{
    public record CreateProfileCommand(long UserId, CreateProfileDto Dto, long ActorUserId)
        : IRequest<ProfileDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

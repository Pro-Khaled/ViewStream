using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Profile.UpdateProfile
{
    public record UpdateProfileCommand(long Id, long UserId, UpdateProfileDto Dto, long ActorUserId)
        : IRequest<ProfileDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

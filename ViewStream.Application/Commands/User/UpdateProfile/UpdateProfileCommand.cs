using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.User.UpdateProfile
{
    // Update own profile
    public record UpdateProfileCommand(long UserId, UpdateUserDto Dto, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

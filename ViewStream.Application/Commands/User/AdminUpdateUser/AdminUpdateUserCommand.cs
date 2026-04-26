using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.User.AdminUpdateUser
{
    // Admin: Update user
    public record AdminUpdateUserCommand(long UserId, AdminUpdateUserDto Dto, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

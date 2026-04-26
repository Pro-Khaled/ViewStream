using MediatR;
using Microsoft.AspNetCore.Identity;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.User.ChangePassword
{
    // Change password
    public record ChangePasswordCommand(long UserId, ChangePasswordDto Dto, long ActorUserId)
        : IRequest<IdentityResult>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

using MediatR;
using ViewStream.Application.Common;

namespace ViewStream.Application.Commands.User.DeleteUser
{
    // Admin: Soft delete user
    public record DeleteUserCommand(long UserId, long AdminUserId) : IRequest<bool>;
}

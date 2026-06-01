using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.UserLibrary.DeleteUserLibraryAdmin
{
    public record DeleteUserLibraryAdminCommand(long Id, long AdminUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => AdminUserId;
    }
}

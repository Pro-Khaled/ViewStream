using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.UserLibrary.DeleteUserLibrary
{
    public record DeleteUserLibraryCommand(long Id, long ProfileId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

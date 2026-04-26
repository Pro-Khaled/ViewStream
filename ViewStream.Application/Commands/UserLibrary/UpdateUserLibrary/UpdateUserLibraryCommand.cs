using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserLibrary.UpdateUserLibrary
{
    public record UpdateUserLibraryCommand(long Id, long ProfileId, UpdateUserLibraryDto Dto, long ActorUserId)
        : IRequest<UserLibraryDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

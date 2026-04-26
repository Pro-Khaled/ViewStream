using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserLibrary.CreateUserLibrary
{
    public record CreateUserLibraryCommand(long ProfileId, CreateUserLibraryDto Dto, long ActorUserId)
        : IRequest<UserLibraryDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

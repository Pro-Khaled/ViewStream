using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserVector.UpsertUserVector
{
    public record UpsertUserVectorCommand(long ProfileId, CreateUpdateUserVectorDto Dto, long ActorUserId)
        : IRequest<UserVectorDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

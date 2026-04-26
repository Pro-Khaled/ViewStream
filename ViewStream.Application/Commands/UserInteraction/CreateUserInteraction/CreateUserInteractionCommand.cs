using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserInteraction.CreateUserInteraction
{
    public record CreateUserInteractionCommand(long ProfileId, CreateUserInteractionDto Dto, long ActorUserId)
        : IRequest<UserInteractionDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

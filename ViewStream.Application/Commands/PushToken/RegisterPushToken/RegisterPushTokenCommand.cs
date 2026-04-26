using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PushToken.RegisterPushToken
{
    public record RegisterPushTokenCommand(long UserId, CreatePushTokenDto Dto, long ActorUserId)
        : IRequest<PushTokenDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

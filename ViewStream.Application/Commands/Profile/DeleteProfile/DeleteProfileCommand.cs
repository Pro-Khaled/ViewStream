using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Profile.DeleteProfile
{
    public record DeleteProfileCommand(long Id, long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

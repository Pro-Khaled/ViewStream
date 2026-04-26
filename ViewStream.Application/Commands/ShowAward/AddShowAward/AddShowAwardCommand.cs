using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ShowAward.AddShowAward
{
    public record AddShowAwardCommand(long ShowId, CreateShowAwardDto Dto, long ActorUserId)
        : IRequest<ShowAwardDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

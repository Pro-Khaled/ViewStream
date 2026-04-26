using MediatR;
using ViewStream.Application.Behaviors;


namespace ViewStream.Application.Commands.Season.RestoreSeason
{
    public record RestoreSeasonCommand(long Id, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

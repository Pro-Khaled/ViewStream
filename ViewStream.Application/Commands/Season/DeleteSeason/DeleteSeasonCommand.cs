using MediatR;
using ViewStream.Application.Behaviors;


namespace ViewStream.Application.Commands.Season.DeleteSeason
{
    public record DeleteSeasonCommand(long Id, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

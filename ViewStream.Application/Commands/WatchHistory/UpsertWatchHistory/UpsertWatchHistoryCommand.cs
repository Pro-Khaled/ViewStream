using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.WatchHistory.UpsertWatchHistory
{
    public record UpsertWatchHistoryCommand(long ProfileId, CreateUpdateWatchHistoryDto Dto, long ActorUserId)
        : IRequest<WatchHistoryDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

using MediatR;

namespace ViewStream.Application.Commands.WatchHistory.DeleteWatchHistory
{
    public record DeleteWatchHistoryCommand(long Id, long AdminUserId) : IRequest<bool>;
}

using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.OfflineDownload.DeleteOfflineDownload
{
    public record DeleteOfflineDownloadCommand(long Id, long ProfileId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

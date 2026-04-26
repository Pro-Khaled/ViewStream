using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.OfflineDownload.CreateOfflineDownload
{
    public record CreateOfflineDownloadCommand(long ProfileId, CreateOfflineDownloadDto Dto, long ActorUserId)
        : IRequest<OfflineDownloadDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

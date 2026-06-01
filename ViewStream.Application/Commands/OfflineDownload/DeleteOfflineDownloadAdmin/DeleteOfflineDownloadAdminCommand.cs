using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.OfflineDownload.DeleteOfflineDownloadAdmin
{
    public record DeleteOfflineDownloadAdminCommand(long Id, long AdminUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => AdminUserId;
    }
}

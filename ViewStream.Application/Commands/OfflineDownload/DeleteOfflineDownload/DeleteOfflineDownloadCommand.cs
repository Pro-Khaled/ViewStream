using MediatR;

namespace ViewStream.Application.Commands.OfflineDownload.DeleteOfflineDownload
{
    public record DeleteOfflineDownloadCommand(long Id, long ProfileId) : IRequest<bool>;

}

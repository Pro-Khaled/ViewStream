using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.OfflineDownload.CreateOfflineDownload
{
    public record CreateOfflineDownloadCommand(long ProfileId, CreateOfflineDownloadDto Dto) : IRequest<OfflineDownloadDto>;

}

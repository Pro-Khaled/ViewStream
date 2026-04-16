using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.OfflineDownload.DeleteOfflineDownload
{
    public class DeleteOfflineDownloadCommandHandler : IRequestHandler<DeleteOfflineDownloadCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOfflineDownloadCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> Handle(DeleteOfflineDownloadCommand request, CancellationToken cancellationToken)
        {
            var download = await _unitOfWork.OfflineDownloads.GetByIdAsync<long>(request.Id, cancellationToken);
            if (download == null || download.ProfileId != request.ProfileId) return false;

            _unitOfWork.OfflineDownloads.Delete(download);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

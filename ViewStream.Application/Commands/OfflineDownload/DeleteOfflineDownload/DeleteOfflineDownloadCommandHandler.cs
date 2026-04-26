using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.OfflineDownload.DeleteOfflineDownload
{
    using OfflineDownload = ViewStream.Domain.Entities.OfflineDownload;
    public class DeleteOfflineDownloadCommandHandler : IRequestHandler<DeleteOfflineDownloadCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteOfflineDownloadCommandHandler> _logger;

        public DeleteOfflineDownloadCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteOfflineDownloadCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteOfflineDownloadCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting offline download Id: {DownloadId} for ProfileId: {ProfileId}",
                request.Id, request.ProfileId);

            var download = await _unitOfWork.OfflineDownloads.GetByIdAsync<long>(request.Id, cancellationToken);
            if (download == null || download.ProfileId != request.ProfileId)
            {
                _logger.LogWarning("Download not found or access denied. Id: {DownloadId}, ProfileId: {ProfileId}",
                    request.Id, request.ProfileId);
                return false;
            }

            var oldValues = _mapper.Map<OfflineDownloadDto>(download);
            _unitOfWork.OfflineDownloads.Delete(download);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<OfflineDownload, object>(
                tableName: "OfflineDownloads",
                recordId: request.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Offline download deleted. Id: {DownloadId}", request.Id);
            return true;
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.OfflineDownload.DeleteOfflineDownloadAdmin
{
    using OfflineDownload = ViewStream.Domain.Entities.OfflineDownload;
    public class DeleteOfflineDownloadAdminCommandHandler : IRequestHandler<DeleteOfflineDownloadAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteOfflineDownloadAdminCommandHandler> _logger;

        public DeleteOfflineDownloadAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteOfflineDownloadAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteOfflineDownloadAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin deleting offline download Id: {DownloadId} by AdminUserId: {AdminUserId}",
                request.Id, request.AdminUserId);

            var download = await _unitOfWork.OfflineDownloads.GetByIdAsync<long>(request.Id, cancellationToken);
            if (download == null)
            {
                _logger.LogWarning("Download not found. Id: {DownloadId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<OfflineDownloadDto>(download);
            _unitOfWork.OfflineDownloads.Delete(download);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<OfflineDownload, object>(
                tableName: "OfflineDownloads",
                recordId: request.Id,
                action: "DELETE_BY_ADMIN",
                oldValues: oldValues,
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Offline download deleted by admin. Id: {DownloadId}", request.Id);
            return true;
        }
    }
}

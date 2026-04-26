using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.OfflineDownload.CreateOfflineDownload
{
    using OfflineDownload = ViewStream.Domain.Entities.OfflineDownload;
    public class CreateOfflineDownloadCommandHandler : IRequestHandler<CreateOfflineDownloadCommand, OfflineDownloadDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateOfflineDownloadCommandHandler> _logger;

        public CreateOfflineDownloadCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateOfflineDownloadCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<OfflineDownloadDto> Handle(CreateOfflineDownloadCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating offline download for ProfileId: {ProfileId}, EpisodeId: {EpisodeId}, DeviceId: {DeviceId}",
                request.ProfileId, request.Dto.EpisodeId, request.Dto.DeviceId);

            var download = new OfflineDownload
            {
                ProfileId = request.ProfileId,
                EpisodeId = request.Dto.EpisodeId,
                DeviceId = request.Dto.DeviceId,
                DownloadQuality = request.Dto.DownloadQuality,
                FilePath = request.Dto.FilePath,
                DownloadedAt = DateTime.UtcNow,
                ExpiresAt = request.Dto.ExpiresAt ?? DateTime.UtcNow.AddDays(7)
            };

            await _unitOfWork.OfflineDownloads.AddAsync(download, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<OfflineDownload, object>(
                tableName: "OfflineDownloads",
                recordId: download.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Offline download created with Id: {DownloadId}", download.Id);

            var result = await _unitOfWork.OfflineDownloads.FindAsync(
                d => d.Id == download.Id,
                include: q => q.Include(d => d.Episode).Include(d => d.Device),
                cancellationToken: cancellationToken);

            return _mapper.Map<OfflineDownloadDto>(result.First());
        }
    }
}

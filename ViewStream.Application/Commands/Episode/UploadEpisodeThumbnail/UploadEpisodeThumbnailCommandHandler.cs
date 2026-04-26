using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Application.Queries.Episode;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.UploadEpisodeThumbnail
{
    using Episode = ViewStream.Domain.Entities.Episode;
    public class UploadEpisodeThumbnailCommandHandler : IRequestHandler<UploadEpisodeThumbnailCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IMediator _mediator;
        private readonly IEpisodeHubClient _hubClient;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UploadEpisodeThumbnailCommandHandler> _logger;

        public UploadEpisodeThumbnailCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IMediator mediator,
            IEpisodeHubClient hubClient,
            IAuditContext auditContext,
            ILogger<UploadEpisodeThumbnailCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _mediator = mediator;
            _hubClient = hubClient;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<string> Handle(UploadEpisodeThumbnailCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Uploading thumbnail for EpisodeId: {EpisodeId}", request.EpisodeId);

            var entity = await _unitOfWork.Episodes.GetByIdAsync<long>(request.EpisodeId, cancellationToken);
            if (entity == null)
                throw new InvalidOperationException("Episode not found.");

            var oldUrl = entity.ThumbnailUrl;
            if (!string.IsNullOrEmpty(oldUrl))
                _fileStorage.DeleteFile(oldUrl);

            var thumbnailUrl = await _fileStorage.SaveThumbnailAsync(request.ThumbnailFile, request.EpisodeId, cancellationToken);
            entity.ThumbnailUrl = thumbnailUrl;
            entity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Episodes.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Episode, object>(
                tableName: "Episodes",
                recordId: entity.Id,
                action: "UPDATE",
                oldValues: new { ThumbnailUrl = oldUrl },
                newValues: new { ThumbnailUrl = thumbnailUrl },
                changedByUserId: request.UploadedByUserId
            );

            _logger.LogInformation("Thumbnail uploaded for EpisodeId: {EpisodeId}, new URL: {Url}", entity.Id, thumbnailUrl);

            var updatedEpisodeDto = await _mediator.Send(new GetEpisodeByIdQuery(request.EpisodeId), cancellationToken);
            await _hubClient.SendEpisodeThumbnailUpdatedAsync(updatedEpisodeDto, cancellationToken);
            await _hubClient.SendEpisodeThumbnailUploadedAsync(request.EpisodeId, thumbnailUrl, request.UploadedByUserId, cancellationToken);

            return thumbnailUrl;
        }
    }
}

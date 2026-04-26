using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Application.Queries.Subtitle;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.UploadSubtitleFile
{
    using Subtitle = ViewStream.Domain.Entities.Subtitle;
    public class UploadSubtitleFileCommandHandler : IRequestHandler<UploadSubtitleFileCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IMediator _mediator;
        private readonly IEpisodeHubClient _hubClient;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UploadSubtitleFileCommandHandler> _logger;

        public UploadSubtitleFileCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IMediator mediator,
            IEpisodeHubClient hubClient,
            IAuditContext auditContext,
            ILogger<UploadSubtitleFileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _mediator = mediator;
            _hubClient = hubClient;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<string> Handle(UploadSubtitleFileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Uploading subtitle file for SubtitleId: {SubtitleId}", request.SubtitleId);

            var subtitle = await _unitOfWork.Subtitles.GetByIdAsync<long>(request.SubtitleId, cancellationToken);
            if (subtitle == null)
                throw new InvalidOperationException("Subtitle not found.");

            var oldUrl = subtitle.SubtitleUrl;
            if (!string.IsNullOrEmpty(oldUrl))
                _fileStorage.DeleteFile(oldUrl);

            var fileUrl = await _fileStorage.SaveSubtitleFileAsync(request.File, request.SubtitleId, cancellationToken);
            subtitle.SubtitleUrl = fileUrl;
            subtitle.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Subtitles.Update(subtitle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Subtitle, object>(
                tableName: "Subtitles",
                recordId: subtitle.Id,
                action: "UPDATE",
                oldValues: new { SubtitleUrl = oldUrl },
                newValues: new { SubtitleUrl = fileUrl },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Subtitle file uploaded for Id: {SubtitleId}, URL: {Url}", subtitle.Id, fileUrl);

            var updatedSubtitleDto = await _mediator.Send(new GetSubtitleByIdQuery(request.SubtitleId), cancellationToken);
            await _hubClient.SendSubtitleFileUpdatedAsync(updatedSubtitleDto, cancellationToken);

            return fileUrl;
        }
    }
}

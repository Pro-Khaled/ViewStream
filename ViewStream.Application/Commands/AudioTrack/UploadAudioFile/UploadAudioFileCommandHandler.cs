using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Application.Queries.AudioTrack;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.UploadAudioFile
{
    using AudioTrack = ViewStream.Domain.Entities.AudioTrack;

    public class UploadAudioFileCommandHandler : IRequestHandler<UploadAudioFileCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IMediator _mediator;
        private readonly IEpisodeHubClient _hubClient;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UploadAudioFileCommandHandler> _logger;

        public UploadAudioFileCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IMediator mediator,
            IEpisodeHubClient hubClient,
            IAuditContext auditContext,
            ILogger<UploadAudioFileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _mediator = mediator;
            _hubClient = hubClient;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<string> Handle(UploadAudioFileCommand request, CancellationToken cancellationToken)
        {
            var audioTrack = await _unitOfWork.AudioTracks.GetByIdAsync<long>(request.AudioTrackId, cancellationToken);
            if (audioTrack == null)
                throw new InvalidOperationException("Audio track not found.");

            var oldUrl = audioTrack.AudioUrl;
            if (!string.IsNullOrEmpty(oldUrl))
                _fileStorage.DeleteFile(oldUrl);

            var fileUrl = await _fileStorage.SaveAudioFileAsync(request.File, request.AudioTrackId, cancellationToken);
            audioTrack.AudioUrl = fileUrl;
            audioTrack.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.AudioTracks.Update(audioTrack);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<AudioTrack, object>(
                tableName: "AudioTracks",
                recordId: audioTrack.Id,
                action: "UPDATE",
                oldValues: new { AudioUrl = oldUrl },
                newValues: new { AudioUrl = fileUrl },
                changedByUserId: request.UploadedByUserId
            );

            _logger.LogInformation("Audio file uploaded for track Id: {AudioTrackId}, new URL: {Url}", audioTrack.Id, fileUrl);

            var updatedAudioDto = await _mediator.Send(new GetAudioTrackByIdQuery(request.AudioTrackId), cancellationToken);
            await _hubClient.SendAudioTrackFileUpdatedAsync(updatedAudioDto, cancellationToken);

            return fileUrl;
        }
    }
}

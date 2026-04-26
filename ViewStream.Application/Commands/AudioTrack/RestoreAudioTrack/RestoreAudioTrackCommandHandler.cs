using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.RestoreAudioTrack
{
    using AudioTrack = ViewStream.Domain.Entities.AudioTrack;

    public class RestoreAudioTrackCommandHandler : IRequestHandler<RestoreAudioTrackCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RestoreAudioTrackCommandHandler> _logger;

        public RestoreAudioTrackCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RestoreAudioTrackCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RestoreAudioTrackCommand request, CancellationToken cancellationToken)
        {
            var audioTrack = await _unitOfWork.AudioTracks.GetByIdAsync<long>(request.Id, cancellationToken);
            if (audioTrack == null || audioTrack.IsDeleted != true)
            {
                _logger.LogWarning("Attempt to restore non-deleted or non-existent audio track Id: {Id}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<AudioTrackDto>(audioTrack);
            audioTrack.IsDeleted = false;
            audioTrack.DeletedAt = null;
            audioTrack.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<AudioTrack, object>(
                tableName: "AudioTracks",
                recordId: audioTrack.Id,
                action: "RESTORE",
                oldValues: oldValues,
                changedByUserId: request.RestoredByUserId
            );

            _logger.LogInformation("Audio track restored with Id: {AudioTrackId}", audioTrack.Id);
            return true;
        }
    }
}

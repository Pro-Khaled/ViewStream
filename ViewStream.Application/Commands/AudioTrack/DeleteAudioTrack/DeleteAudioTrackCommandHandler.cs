using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.DeleteAudioTrack
{
    using AudioTrack = ViewStream.Domain.Entities.AudioTrack;

    public class DeleteAudioTrackCommandHandler : IRequestHandler<DeleteAudioTrackCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteAudioTrackCommandHandler> _logger;

        public DeleteAudioTrackCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteAudioTrackCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteAudioTrackCommand request, CancellationToken cancellationToken)
        {
            var audioTrack = await _unitOfWork.AudioTracks.GetByIdAsync<long>(request.Id, cancellationToken);
            if (audioTrack == null || audioTrack.IsDeleted == true)
            {
                _logger.LogWarning("Attempt to delete non-existent or already deleted audio track Id: {Id}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<AudioTrackDto>(audioTrack);
            audioTrack.IsDeleted = true;
            audioTrack.DeletedAt = DateTime.UtcNow;
            audioTrack.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<AudioTrack, object>(
                tableName: "AudioTracks",
                recordId: audioTrack.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.DeletedByUserId
            );

            _logger.LogInformation("Audio track soft-deleted with Id: {AudioTrackId}", audioTrack.Id);
            return true;
        }
    }
}

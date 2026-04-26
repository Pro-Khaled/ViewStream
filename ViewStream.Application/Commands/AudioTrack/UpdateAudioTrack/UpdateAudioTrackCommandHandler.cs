using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.UpdateAudioTrack
{
    using AudioTrack = ViewStream.Domain.Entities.AudioTrack;

    public class UpdateAudioTrackCommandHandler : IRequestHandler<UpdateAudioTrackCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateAudioTrackCommandHandler> _logger;

        public UpdateAudioTrackCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateAudioTrackCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateAudioTrackCommand request, CancellationToken cancellationToken)
        {
            var audioTrack = await _unitOfWork.AudioTracks.GetByIdAsync<long>(request.Id, cancellationToken);
            if (audioTrack == null || audioTrack.IsDeleted == true)
            {
                _logger.LogWarning("Attempt to update non-existent or deleted audio track Id: {Id}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<AudioTrackDto>(audioTrack);
            _mapper.Map(request.Dto, audioTrack);
            audioTrack.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.AudioTracks.Update(audioTrack);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<AudioTrack, UpdateAudioTrackDto>(
                tableName: "AudioTracks",
                recordId: audioTrack.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.UpdatedByUserId
            );

            _logger.LogInformation("Audio track updated with Id: {AudioTrackId}", audioTrack.Id);
            return true;
        }
    }
}
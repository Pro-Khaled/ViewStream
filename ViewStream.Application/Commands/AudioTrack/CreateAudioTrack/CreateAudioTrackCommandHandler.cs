using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.CreateAudioTrack
{
    using AudioTrack = ViewStream.Domain.Entities.AudioTrack;

    public class CreateAudioTrackCommandHandler : IRequestHandler<CreateAudioTrackCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateAudioTrackCommandHandler> _logger;

        public CreateAudioTrackCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateAudioTrackCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<long> Handle(CreateAudioTrackCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating audio track for EpisodeId: {EpisodeId}, Language: {LanguageCode}",
                request.Dto.EpisodeId, request.Dto.LanguageCode);

            var audioTrack = _mapper.Map<AudioTrack>(request.Dto);
            audioTrack.CreatedAt = DateTime.UtcNow;
            audioTrack.IsDeleted = false;

            await _unitOfWork.AudioTracks.AddAsync(audioTrack, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<AudioTrack, CreateAudioTrackDto>(
                tableName: "AudioTracks",
                recordId: audioTrack.Id,
                action: "INSERT",
                newValues: request.Dto,
                changedByUserId: request.CreatedByUserId
            );

            _logger.LogInformation("Audio track created with Id: {AudioTrackId}", audioTrack.Id);
            return audioTrack.Id;
        }
    }
}

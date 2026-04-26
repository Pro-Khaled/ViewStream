using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.DeleteEpisode
{
    using Episode = ViewStream.Domain.Entities.Episode;
    public class DeleteEpisodeCommandHandler : IRequestHandler<DeleteEpisodeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteEpisodeCommandHandler> _logger;

        public DeleteEpisodeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteEpisodeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteEpisodeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting episode Id: {EpisodeId}", request.Id);

            var episode = await _unitOfWork.Episodes.GetByIdAsync<long>(request.Id, cancellationToken);
            if (episode == null || episode.IsDeleted == true)
            {
                _logger.LogWarning("Episode not found or already deleted. Id: {EpisodeId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<EpisodeDto>(episode);
            episode.IsDeleted = true;
            episode.DeletedAt = DateTime.UtcNow;
            episode.UpdatedAt = DateTime.UtcNow;

            var audioTracks = await _unitOfWork.AudioTracks.FindAsync(a => a.EpisodeId == request.Id, cancellationToken: cancellationToken);
            foreach (var audio in audioTracks)
                audio.IsDeleted = true;

            var subtitles = await _unitOfWork.Subtitles.FindAsync(s => s.EpisodeId == request.Id, cancellationToken: cancellationToken);
            foreach (var subtitle in subtitles)
                subtitle.IsDeleted = true;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Episode, object>(
                tableName: "Episodes",
                recordId: episode.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.DeletedByUserId
            );

            _logger.LogInformation("Episode soft-deleted with Id: {EpisodeId}", episode.Id);
            return true;
        }
    }
}

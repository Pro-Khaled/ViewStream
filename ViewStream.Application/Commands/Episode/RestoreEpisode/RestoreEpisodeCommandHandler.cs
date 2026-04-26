using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.RestoreEpisode
{
    using Episode = ViewStream.Domain.Entities.Episode;
    public class RestoreEpisodeCommandHandler : IRequestHandler<RestoreEpisodeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RestoreEpisodeCommandHandler> _logger;

        public RestoreEpisodeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RestoreEpisodeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RestoreEpisodeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Restoring episode Id: {EpisodeId}", request.Id);

            var episode = await _unitOfWork.Episodes.GetByIdAsync<long>(request.Id, cancellationToken);
            if (episode == null || episode.IsDeleted != true)
            {
                _logger.LogWarning("Episode not found or not deleted. Id: {EpisodeId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<EpisodeDto>(episode);
            episode.IsDeleted = false;
            episode.DeletedAt = null;
            episode.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Episode, object>(
                tableName: "Episodes",
                recordId: episode.Id,
                action: "RESTORE",
                oldValues: oldValues,
                changedByUserId: request.RestoredByUserId
            );

            _logger.LogInformation("Episode restored with Id: {EpisodeId}", episode.Id);
            return true;
        }
    }
}

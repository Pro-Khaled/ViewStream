using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.UpdateEpisode
{
    using Episode = ViewStream.Domain.Entities.Episode;
    public class UpdateEpisodeCommandHandler : IRequestHandler<UpdateEpisodeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateEpisodeCommandHandler> _logger;

        public UpdateEpisodeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateEpisodeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateEpisodeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating episode Id: {EpisodeId}", request.Id);

            var episode = await _unitOfWork.Episodes.GetByIdAsync<long>(request.Id, cancellationToken);
            if (episode == null || episode.IsDeleted == true)
            {
                _logger.LogWarning("Episode not found or already deleted. Id: {EpisodeId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<EpisodeDto>(episode);
            _mapper.Map(request.Dto, episode);
            episode.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Episodes.Update(episode);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Episode, object>(
                tableName: "Episodes",
                recordId: episode.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.UpdatedByUserId
            );

            _logger.LogInformation("Episode updated with Id: {EpisodeId}", episode.Id);
            return true;
        }
    }
}

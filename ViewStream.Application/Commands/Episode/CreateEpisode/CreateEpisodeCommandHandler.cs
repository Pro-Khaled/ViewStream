using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.CreateEpisode
{
    using Episode = Domain.Entities.Episode;
    public class CreateEpisodeCommandHandler : IRequestHandler<CreateEpisodeCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateEpisodeCommandHandler> _logger;

        public CreateEpisodeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileStorageService fileStorage,
            IAuditContext auditContext,
            ILogger<CreateEpisodeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorage = fileStorage;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<long> Handle(CreateEpisodeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating episode for SeasonId: {SeasonId}", request.Dto.SeasonId);

            var episode = _mapper.Map<Episode>(request.Dto);
            episode.CreatedAt = DateTime.UtcNow;
            episode.IsDeleted = false;

            await _unitOfWork.Episodes.AddAsync(episode, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Handle video file upload
            if (request.Dto.VideoFile != null)
            {
                var videoUrl = await _fileStorage.SaveVideoAsync(request.Dto.VideoFile, episode.Id, cancellationToken);
                episode.VideoUrl = videoUrl;
                _unitOfWork.Episodes.Update(episode);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            _auditContext.SetAudit<Episode, object>(
                tableName: "Episodes",
                recordId: episode.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.CreatedByUserId
            );

            _logger.LogInformation("Episode created with Id: {EpisodeId}", episode.Id);
            return episode.Id;
        }
    }
}

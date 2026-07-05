using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Contracts;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
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
        private readonly IMessageBus _messageBus;
        private readonly ILogger<CreateEpisodeCommandHandler> _logger;

        public CreateEpisodeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileStorageService fileStorage,
            IAuditContext auditContext,
            IMessageBus messageBus,
            ILogger<CreateEpisodeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorage = fileStorage;
            _auditContext = auditContext;
            _messageBus = messageBus;
            _logger = logger;
        }

        public async Task<long> Handle(CreateEpisodeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating episode for SeasonId: {SeasonId}", request.Dto.SeasonId);

            // Validate season and associated show
            var season = await _unitOfWork.Repository<Season>().GetByIdAsync<long>(request.Dto.SeasonId, cancellationToken);
            if (season == null || season.IsDeleted == true)
                throw new ArgumentException("Season not found or is inactive.");

            var show = await _unitOfWork.Shows.GetByIdAsync<long>(season.ShowId, cancellationToken);
            if (show == null || show.IsDeleted == true)
                throw new ArgumentException("Associated show was not found or is deleted.");

            // 1. Save raw video file (if provided)
            string? rawVideoUrl = request.Dto.VideoUrl;
            if (request.Dto.VideoFile != null)
            {
                rawVideoUrl = await _fileStorage.SaveVideoAsync(request.Dto.VideoFile, 0, cancellationToken);
                // Note: episode.Id not available yet; save with temporary ID? Better to save episode first then upload.
                // We'll adjust: save episode first, then upload, then update.
            }

            // 2. Create episode entity (without HLS info)
            var episode = _mapper.Map<Episode>(request.Dto);
            episode.CreatedAt = DateTime.UtcNow;
            episode.IsDeleted = false;
            episode.VideoUrl = rawVideoUrl ?? string.Empty;     // temporary raw URL
            episode.HlsMasterUrl = null;
            episode.DurationSeconds = null;
            episode.ThumbnailUrl = null;

            await _unitOfWork.Episodes.AddAsync(episode, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 3. If video file was uploaded, we need to update the episode with correct file URL
            //    (since SaveVideoAsync might use episode.Id for folder structure)
            if (request.Dto.VideoFile != null && string.IsNullOrEmpty(rawVideoUrl))
            {
                // Re-upload using the actual episode.Id
                rawVideoUrl = await _fileStorage.SaveVideoAsync(request.Dto.VideoFile, episode.Id, cancellationToken);
                episode.VideoUrl = rawVideoUrl;
                _unitOfWork.Episodes.Update(episode);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // 4. Create a VideoProcessingJob (if raw video exists)
            if (!string.IsNullOrEmpty(episode.VideoUrl))
            {
                // Check for existing pending job (optional, but safe)
                var existingJob = await _unitOfWork.VideoProcessingJobs.GetPendingByEpisodeIdAsync(episode.Id);
                if (existingJob == null)
                {
                    var job = new VideoProcessingJob
                    {
                        EpisodeId = episode.Id,
                        SourceFileUrl = episode.VideoUrl,
                        Status = "Pending",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.VideoProcessingJobs.AddAsync(job, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // 5. Publish message to RabbitMQ
                    await _messageBus.Publish(new TranscodeVideoMessage { JobId = job.Id });
                    _logger.LogInformation("Job {JobId} created and published for episode {EpisodeId}", job.Id, episode.Id);
                }
                else
                {
                    _logger.LogWarning("Pending job already exists for episode {EpisodeId}", episode.Id);
                }
            }

            // Audit log
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
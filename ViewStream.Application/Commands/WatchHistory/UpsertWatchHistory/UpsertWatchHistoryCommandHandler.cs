using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Events;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchHistory.UpsertWatchHistory
{
    using WatchHistory = ViewStream.Domain.Entities.WatchHistory;
    public class UpsertWatchHistoryCommandHandler : IRequestHandler<UpsertWatchHistoryCommand, WatchHistoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly IPublisher _publisher;
        private readonly ILogger<UpsertWatchHistoryCommandHandler> _logger;

        public UpsertWatchHistoryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            IPublisher publisher,
            ILogger<UpsertWatchHistoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task<WatchHistoryDto> Handle(UpsertWatchHistoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Upserting watch history for ProfileId: {ProfileId}, EpisodeId: {EpisodeId}",
                request.ProfileId, request.Dto.EpisodeId);

            var existing = await _unitOfWork.WatchHistories.FindAsync(
                wh => wh.ProfileId == request.ProfileId && wh.EpisodeId == request.Dto.EpisodeId,
                cancellationToken: cancellationToken);

            var history = existing.FirstOrDefault();
            bool isNew = history == null;
            int? oldProgress = history?.ProgressSeconds;
            bool? oldCompleted = history?.Completed;

            if (isNew)
            {
                history = new WatchHistory
                {
                    ProfileId = request.ProfileId,
                    EpisodeId = request.Dto.EpisodeId,
                    ProgressSeconds = request.Dto.ProgressSeconds ?? 0,
                    Completed = request.Dto.Completed ?? false,
                    WatchedAt = DateTime.UtcNow
                };
                await _unitOfWork.WatchHistories.AddAsync(history, cancellationToken);
            }
            else
            {
                // Monotonic progress enforcement: only update if new progress > old progress
                var newProgress = request.Dto.ProgressSeconds ?? history.ProgressSeconds;
                if (newProgress > (history.ProgressSeconds ?? 0))
                    history.ProgressSeconds = newProgress;

                history.Completed = request.Dto.Completed ?? history.Completed;
                history.WatchedAt = DateTime.UtcNow;
                _unitOfWork.WatchHistories.Update(history);
            }

            // Auto-completion detection at 90% threshold
            if (history.Completed != true && history.ProgressSeconds > 0)
            {
                var episodes = await _unitOfWork.Episodes.FindAsync(
                    e => e.Id == request.Dto.EpisodeId,
                    asNoTracking: true,
                    cancellationToken: cancellationToken);
                var episode = episodes.FirstOrDefault();

                if (episode?.DurationSeconds > 0 &&
                    history.ProgressSeconds >= (int)(episode.DurationSeconds * 0.9))
                {
                    history.Completed = true;
                    history.CompletedAt = DateTime.UtcNow;
                    _logger.LogInformation("Auto-completed: Profile {ProfileId}, Episode {EpisodeId} at {Progress}/{Duration}s",
                        request.ProfileId, request.Dto.EpisodeId, history.ProgressSeconds, episode.DurationSeconds);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<WatchHistory, object>(
                tableName: "WatchHistories",
                recordId: history.Id,
                action: isNew ? "INSERT" : "UPDATE",
                oldValues: isNew ? null : new { oldProgress, oldCompleted },
                newValues: new { history.ProgressSeconds, history.Completed },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Watch history {Action} for ProfileId: {ProfileId}, EpisodeId: {EpisodeId}",
                isNew ? "created" : "updated", request.ProfileId, request.Dto.EpisodeId);

            // Publish domain event if episode was just completed
            if (history.Completed == true && oldCompleted != true)
            {
                var ep = await _unitOfWork.Episodes.FindAsync(
                    e => e.Id == request.Dto.EpisodeId,
                    include: q => q.Include(e => e.Season),
                    asNoTracking: true,
                    cancellationToken: cancellationToken);
                var episode = ep.FirstOrDefault();
                if (episode != null)
                {
                    await _publisher.Publish(
                        new EpisodeCompletedEvent(request.ProfileId, episode.Id, episode.Season.ShowId),
                        cancellationToken);
                }
            }

            var result = await _unitOfWork.WatchHistories.FindAsync(
                wh => wh.Id == history.Id,
                include: q => q.Include(wh => wh.Profile)
                               .Include(wh => wh.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show),
                cancellationToken: cancellationToken);

            return _mapper.Map<WatchHistoryDto>(result.First());
        }
    }
}

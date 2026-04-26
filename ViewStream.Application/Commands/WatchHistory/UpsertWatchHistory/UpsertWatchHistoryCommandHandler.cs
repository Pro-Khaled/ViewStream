using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchHistory.UpsertWatchHistory
{
    using WatchHistory = ViewStream.Domain.Entities.WatchHistory;
    public class UpsertWatchHistoryCommandHandler : IRequestHandler<UpsertWatchHistoryCommand, WatchHistoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpsertWatchHistoryCommandHandler> _logger;

        public UpsertWatchHistoryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpsertWatchHistoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
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
                history.ProgressSeconds = request.Dto.ProgressSeconds ?? history.ProgressSeconds;
                history.Completed = request.Dto.Completed ?? history.Completed;
                history.WatchedAt = DateTime.UtcNow;
                _unitOfWork.WatchHistories.Update(history);
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

            var result = await _unitOfWork.WatchHistories.FindAsync(
                wh => wh.Id == history.Id,
                include: q => q.Include(wh => wh.Profile)
                               .Include(wh => wh.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show),
                cancellationToken: cancellationToken);

            return _mapper.Map<WatchHistoryDto>(result.First());
        }
    }
}

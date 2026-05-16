using AutoMapper;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PlaybackEvent.PurgeOldPlaybackEvents
{
    public class PurgeOldPlaybackEventsCommandHandler : IRequestHandler<PurgeOldPlaybackEventsCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<PurgeOldPlaybackEventsCommandHandler> _logger;

        public PurgeOldPlaybackEventsCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<PurgeOldPlaybackEventsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<int> Handle(PurgeOldPlaybackEventsCommand request, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-request.DaysToKeep);
            
            var deletedCount = await _unitOfWork.PlaybackEvents.GetQueryable()
                .Where(x => x.CreatedAt < cutoffDate)
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogInformation("Purged {Count} old playback events (older than {CutoffDate})", deletedCount, cutoffDate);
            return deletedCount;
        }
    }
}


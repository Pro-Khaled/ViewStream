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

namespace ViewStream.Application.Commands.WatchHistory.PurgeOldWatchHistories
{
    public class PurgeOldWatchHistoriesCommandHandler : IRequestHandler<PurgeOldWatchHistoriesCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<PurgeOldWatchHistoriesCommandHandler> _logger;

        public PurgeOldWatchHistoriesCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<PurgeOldWatchHistoriesCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<int> Handle(PurgeOldWatchHistoriesCommand request, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-request.DaysToKeep);
            
            var deletedCount = await _unitOfWork.WatchHistories.GetQueryable()
                .Where(x => x.WatchedAt < cutoffDate)
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogInformation("Purged {Count} old watch histories (older than {CutoffDate})", deletedCount, cutoffDate);
            return deletedCount;
        }
    }
}


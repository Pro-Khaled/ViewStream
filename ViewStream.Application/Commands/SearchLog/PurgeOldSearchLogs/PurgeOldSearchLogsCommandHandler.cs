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

namespace ViewStream.Application.Commands.SearchLog.PurgeOldSearchLogs
{
    public class PurgeOldSearchLogsCommandHandler : IRequestHandler<PurgeOldSearchLogsCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<PurgeOldSearchLogsCommandHandler> _logger;

        public PurgeOldSearchLogsCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<PurgeOldSearchLogsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<int> Handle(PurgeOldSearchLogsCommand request, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-request.DaysToKeep);
            
            var deletedCount = await _unitOfWork.SearchLogs.GetQueryable()
                .Where(x => x.SearchAt < cutoffDate)
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogInformation("Purged {Count} old search logs (older than {CutoffDate})", deletedCount, cutoffDate);
            return deletedCount;
        }
    }
}


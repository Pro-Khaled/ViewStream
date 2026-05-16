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

namespace ViewStream.Application.Commands.ErrorLog.PurgeOldErrorLogs
{
    public class PurgeOldErrorLogsCommandHandler : IRequestHandler<PurgeOldErrorLogsCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<PurgeOldErrorLogsCommandHandler> _logger;

        public PurgeOldErrorLogsCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<PurgeOldErrorLogsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<int> Handle(PurgeOldErrorLogsCommand request, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-request.DaysToKeep);
            
            var deletedCount = await _unitOfWork.ErrorLogs.GetQueryable()
                .Where(x => x.OccurredAt < cutoffDate)
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogInformation("Purged {Count} old error logs (older than {CutoffDate})", deletedCount, cutoffDate);
            return deletedCount;
        }
    }
}


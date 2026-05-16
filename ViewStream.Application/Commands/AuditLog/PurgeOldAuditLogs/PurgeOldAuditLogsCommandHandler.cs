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

namespace ViewStream.Application.Commands.AuditLog.PurgeOldAuditLogs
{
    public class PurgeOldAuditLogsCommandHandler : IRequestHandler<PurgeOldAuditLogsCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<PurgeOldAuditLogsCommandHandler> _logger;

        public PurgeOldAuditLogsCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<PurgeOldAuditLogsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<int> Handle(PurgeOldAuditLogsCommand request, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-request.DaysToKeep);
            
            // Using ExecuteDeleteAsync for bulk deletion without loading into memory
            var deletedCount = await _unitOfWork.AuditLogs.GetQueryable()
                .Where(x => x.ChangedAt < cutoffDate)
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogInformation("Purged {Count} old audit logs (older than {CutoffDate})", deletedCount, cutoffDate);
            return deletedCount;
        }
    }
}


using AutoMapper;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Exceptions;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AuditLog.DeleteAuditLog
{
    public class DeleteAuditLogCommandHandler : IRequestHandler<DeleteAuditLogCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteAuditLogCommandHandler> _logger;

        public DeleteAuditLogCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteAuditLogCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteAuditLogCommand request, CancellationToken cancellationToken)
        {
            var log = await _unitOfWork.AuditLogs.GetByIdAsync<long>(request.Id, cancellationToken);
            if (log == null)
            {
                throw new NotFoundException("AuditLog", request.Id);
            }

            _unitOfWork.AuditLogs.Delete(log);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Audit log hard-deleted with Id: {LogId}", log.Id);
            return true;
        }
    }
}

using AutoMapper;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Exceptions;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ErrorLog.DeleteErrorLog
{
    public class DeleteErrorLogCommandHandler : IRequestHandler<DeleteErrorLogCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteErrorLogCommandHandler> _logger;

        public DeleteErrorLogCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteErrorLogCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteErrorLogCommand request, CancellationToken cancellationToken)
        {
            var log = await _unitOfWork.ErrorLogs.GetByIdAsync<long>(request.Id, cancellationToken);
            if (log == null)
            {
                throw new NotFoundException("ErrorLog", request.Id);
            }

            _unitOfWork.ErrorLogs.Delete(log);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Error log hard-deleted with Id: {LogId}", log.Id);
            return true;
        }
    }
}


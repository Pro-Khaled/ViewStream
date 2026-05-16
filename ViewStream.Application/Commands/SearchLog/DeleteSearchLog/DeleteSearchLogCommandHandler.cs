using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Exceptions;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SearchLog.DeleteSearchLog
{
    public class DeleteSearchLogCommandHandler : IRequestHandler<DeleteSearchLogCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteSearchLogCommandHandler> _logger;

        public DeleteSearchLogCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteSearchLogCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteSearchLogCommand request, CancellationToken cancellationToken)
        {
            var log = await _unitOfWork.SearchLogs.GetByIdAsync<long>(request.Id, cancellationToken);
            if (log == null)
            {
                throw new NotFoundException("SearchLog", request.Id);
            }

            _unitOfWork.SearchLogs.Delete(log);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Search log hard-deleted with Id: {LogId} by admin: {AdminId}", log.Id, request.AdminUserId);
            return true;
        }
    }
}

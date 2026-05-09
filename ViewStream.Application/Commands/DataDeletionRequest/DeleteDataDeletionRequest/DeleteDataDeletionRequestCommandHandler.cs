using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.DataDeletionRequest.DeleteDataDeletionRequest
{
    using DataDeletionRequest = ViewStream.Domain.Entities.DataDeletionRequest;

    public class DeleteDataDeletionRequestCommandHandler
        : IRequestHandler<DeleteDataDeletionRequestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteDataDeletionRequestCommandHandler> _logger;

        public DeleteDataDeletionRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<DeleteDataDeletionRequestCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteDataDeletionRequestCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserId: {UserId} cancelling deletion request {RequestId}",
                request.UserId, request.Id);

            var records = await _unitOfWork.DataDeletionRequests.FindAsync(
                d => d.Id == request.Id && d.UserId == request.UserId,
                asNoTracking: false,
                cancellationToken: cancellationToken);

            var record = records.FirstOrDefault();
            if (record == null)
            {
                _logger.LogWarning("Deletion request {RequestId} not found for UserId: {UserId}",
                    request.Id, request.UserId);
                return false;
            }

            if (record.Status != "pending")
            {
                _logger.LogWarning("Deletion request {RequestId} cannot be cancelled — status is {Status}",
                    request.Id, record.Status);
                return false;
            }

            _unitOfWork.DataDeletionRequests.Delete(record);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<DataDeletionRequest, object>(
                tableName: "DataDeletionRequests",
                recordId: record.Id,
                action: "DELETE",
                oldValues: new { record.UserId, record.Status },
                newValues: null,
                changedByUserId: request.UserId);

            _logger.LogInformation("Deletion request {RequestId} cancelled by UserId: {UserId}",
                request.Id, request.UserId);
            return true;
        }
    }
}

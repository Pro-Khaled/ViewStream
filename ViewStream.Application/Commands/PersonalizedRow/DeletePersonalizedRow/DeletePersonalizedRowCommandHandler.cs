using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonalizedRow.DeletePersonalizedRow
{
    using PersonalizedRow = ViewStream.Domain.Entities.PersonalizedRow;
    public class DeletePersonalizedRowCommandHandler : IRequestHandler<DeletePersonalizedRowCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeletePersonalizedRowCommandHandler> _logger;

        public DeletePersonalizedRowCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<DeletePersonalizedRowCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePersonalizedRowCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting personalized row '{RowName}' for ProfileId: {ProfileId}",
                request.RowName, request.ProfileId);

            var rows = await _unitOfWork.PersonalizedRows.FindAsync(
                r => r.ProfileId == request.ProfileId && r.RowName == request.RowName,
                cancellationToken: cancellationToken);

            var row = rows.FirstOrDefault();
            if (row == null)
            {
                _logger.LogWarning("Row not found: '{RowName}' for ProfileId: {ProfileId}", request.RowName, request.ProfileId);
                return false;
            }

            var oldValues = new { row.ProfileId, row.RowName, row.ShowIdsJson, row.GeneratedAt };
            _unitOfWork.PersonalizedRows.Delete(row);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PersonalizedRow, object>(
                tableName: "PersonalizedRows",
                recordId: row.ProfileId.GetHashCode() ^ row.RowName.GetHashCode(),
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Personalized row '{RowName}' deleted for ProfileId: {ProfileId}",
                request.RowName, request.ProfileId);
            return true;
        }
    }
}

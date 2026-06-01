using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchParty.DeleteWatchPartyAdmin
{
    using WatchParty = ViewStream.Domain.Entities.WatchParty;
    public class DeleteWatchPartyAdminCommandHandler : IRequestHandler<DeleteWatchPartyAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteWatchPartyAdminCommandHandler> _logger;

        public DeleteWatchPartyAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<DeleteWatchPartyAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteWatchPartyAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin deleting watch party Id: {WatchPartyId} by AdminUserId: {AdminUserId}", request.Id, request.AdminUserId);

            var watchParty = await _unitOfWork.WatchParties.GetByIdAsync<long>(request.Id, cancellationToken);
            if (watchParty == null)
            {
                _logger.LogWarning("Watch party not found. Id: {WatchPartyId}", request.Id);
                return false;
            }

            var oldValues = new
            {
                watchParty.HostProfileId,
                watchParty.EpisodeId,
                watchParty.PartyCode,
                watchParty.StartedAt,
                watchParty.EndedAt,
                watchParty.IsActive
            };

            // First remove participants if any (cascading delete if not automatic, to be safe)
            var participants = await _unitOfWork.WatchPartyParticipants.FindAsync(
                p => p.PartyId == request.Id,
                cancellationToken: cancellationToken);
            if (participants.Any())
            {
                _unitOfWork.WatchPartyParticipants.DeleteRange(participants);
            }

            _unitOfWork.WatchParties.Delete(watchParty);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<WatchParty, object>(
                tableName: "WatchParties",
                recordId: request.Id,
                action: "DELETE_BY_ADMIN",
                oldValues: oldValues,
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Watch party deleted by admin. Id: {WatchPartyId}", request.Id);
            return true;
        }
    }
}

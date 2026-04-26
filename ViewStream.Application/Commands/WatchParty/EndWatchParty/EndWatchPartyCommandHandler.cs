using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchParty.DeleteWatchParty
{
    using WatchParty = ViewStream.Domain.Entities.WatchParty;
    public class EndWatchPartyCommandHandler : IRequestHandler<EndWatchPartyCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<EndWatchPartyCommandHandler> _logger;

        public EndWatchPartyCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<EndWatchPartyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(EndWatchPartyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ending watch party Id: {PartyId}", request.Id);

            var party = await _unitOfWork.WatchParties.GetByIdAsync<long>(request.Id, cancellationToken);
            if (party == null || party.HostProfileId != request.ProfileId)
            {
                _logger.LogWarning("Party not found or not the host. Id: {PartyId}", request.Id);
                return false;
            }

            var oldValues = new { party.IsActive, party.EndedAt };
            party.IsActive = false;
            party.EndedAt = DateTime.UtcNow;

            _unitOfWork.WatchParties.Update(party);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<WatchParty, object>(
                tableName: "WatchParties",
                recordId: party.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: new { party.IsActive, party.EndedAt },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Watch party ended. Id: {PartyId}", party.Id);
            return true;
        }
    }
}

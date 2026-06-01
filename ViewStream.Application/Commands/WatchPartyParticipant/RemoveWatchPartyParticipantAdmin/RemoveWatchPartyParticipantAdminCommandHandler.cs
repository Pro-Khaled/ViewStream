using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchPartyParticipant.RemoveWatchPartyParticipantAdmin
{
    using WatchPartyParticipant = ViewStream.Domain.Entities.WatchPartyParticipant;
    public class RemoveWatchPartyParticipantAdminCommandHandler : IRequestHandler<RemoveWatchPartyParticipantAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RemoveWatchPartyParticipantAdminCommandHandler> _logger;

        public RemoveWatchPartyParticipantAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RemoveWatchPartyParticipantAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RemoveWatchPartyParticipantAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin removing participant PartyId: {PartyId}, ProfileId: {ProfileId} by AdminUserId: {AdminUserId}",
                request.PartyId, request.ProfileId, request.AdminUserId);

            var participants = await _unitOfWork.WatchPartyParticipants.FindAsync(
                p => p.PartyId == request.PartyId && p.ProfileId == request.ProfileId,
                cancellationToken: cancellationToken);

            var participant = participants.FirstOrDefault();
            if (participant == null)
            {
                _logger.LogWarning("Participant not found. PartyId: {PartyId}, ProfileId: {ProfileId}", request.PartyId, request.ProfileId);
                return false;
            }

            var oldValues = _mapper.Map<WatchPartyParticipantDto>(participant);
            _unitOfWork.WatchPartyParticipants.Delete(participant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<WatchPartyParticipant, object>(
                tableName: "WatchPartyParticipants",
                recordId: participant.PartyId.GetHashCode() ^ participant.ProfileId,
                action: "DELETE_BY_ADMIN",
                oldValues: oldValues,
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Participant removed by admin. PartyId: {PartyId}, ProfileId: {ProfileId}", request.PartyId, request.ProfileId);
            return true;
        }
    }
}

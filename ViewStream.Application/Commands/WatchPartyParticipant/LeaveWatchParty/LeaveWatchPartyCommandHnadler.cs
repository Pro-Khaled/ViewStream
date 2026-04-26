using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchPartyParticipant.LeaveWatchParty
{
    using WatchPartyParticipant = ViewStream.Domain.Entities.WatchPartyParticipant;
    public class LeaveWatchPartyCommandHandler : IRequestHandler<LeaveWatchPartyCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<LeaveWatchPartyCommandHandler> _logger;

        public LeaveWatchPartyCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<LeaveWatchPartyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(LeaveWatchPartyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Profile {ProfileId} leaving watch party {PartyId}", request.ProfileId, request.PartyId);

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
            participant.LeftAt = DateTime.UtcNow;
            _unitOfWork.WatchPartyParticipants.Update(participant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<WatchPartyParticipant, object>(
                tableName: "WatchPartyParticipants",
                recordId: participant.PartyId.GetHashCode() ^ participant.ProfileId,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: new { participant.LeftAt },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Profile {ProfileId} left watch party {PartyId}", request.ProfileId, request.PartyId);
            return true;
        }
    }
}

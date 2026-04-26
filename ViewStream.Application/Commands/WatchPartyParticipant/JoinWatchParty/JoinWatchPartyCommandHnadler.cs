using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchPartyParticipant.JoinWatchParty
{
    using WatchPartyParticipant = ViewStream.Domain.Entities.WatchPartyParticipant;
    public class JoinWatchPartyCommandHandler : IRequestHandler<JoinWatchPartyCommand, WatchPartyParticipantDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<JoinWatchPartyCommandHandler> _logger;

        public JoinWatchPartyCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<JoinWatchPartyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<WatchPartyParticipantDto> Handle(JoinWatchPartyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Profile {ProfileId} joining watch party {PartyId}", request.ProfileId, request.PartyId);

            var party = await _unitOfWork.WatchParties.GetByIdAsync<long>(request.PartyId, cancellationToken);
            if (party == null || party.IsActive != true)
                throw new InvalidOperationException("Watch party not found or inactive.");

            var existing = await _unitOfWork.WatchPartyParticipants.FindAsync(
                p => p.PartyId == request.PartyId && p.ProfileId == request.ProfileId,
                cancellationToken: cancellationToken);

            var participant = existing.FirstOrDefault();
            bool isNew = participant == null;
            DateTime? oldLeftAt = participant?.LeftAt;

            if (isNew)
            {
                participant = new WatchPartyParticipant
                {
                    PartyId = request.PartyId,
                    ProfileId = request.ProfileId,
                    JoinedAt = DateTime.UtcNow
                };
                await _unitOfWork.WatchPartyParticipants.AddAsync(participant, cancellationToken);
            }
            else
            {
                participant.LeftAt = null; // re-join clears leave time
                _unitOfWork.WatchPartyParticipants.Update(participant);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<WatchPartyParticipant, object>(
                tableName: "WatchPartyParticipants",
                recordId: participant.PartyId.GetHashCode() ^ participant.ProfileId,
                action: isNew ? "INSERT" : "UPDATE",
                oldValues: isNew ? null : new { LeftAt = oldLeftAt },
                newValues: new { participant.PartyId, participant.ProfileId, participant.JoinedAt, participant.LeftAt },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Profile {ProfileId} {Action} watch party {PartyId}",
                request.ProfileId, isNew ? "joined" : "rejoined", request.PartyId);

            var result = await _unitOfWork.WatchPartyParticipants.FindAsync(
                p => p.PartyId == participant.PartyId && p.ProfileId == participant.ProfileId,
                include: q => q.Include(p => p.Profile),
                cancellationToken: cancellationToken);

            return _mapper.Map<WatchPartyParticipantDto>(result.First());
        }
    }
}

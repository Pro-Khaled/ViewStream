using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchParty.UpdateWatchParty
{
    using WatchParty = ViewStream.Domain.Entities.WatchParty;
    public class UpdateWatchPartyCommandHandler : IRequestHandler<UpdateWatchPartyCommand, WatchPartyDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateWatchPartyCommandHandler> _logger;

        public UpdateWatchPartyCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateWatchPartyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<WatchPartyDto?> Handle(UpdateWatchPartyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating watch party Id: {PartyId}", request.Id);

            var party = await _unitOfWork.WatchParties.GetByIdAsync<long>(request.Id, cancellationToken);
            if (party == null || party.HostProfileId != request.ProfileId)
            {
                _logger.LogWarning("Party not found or not the host. Id: {PartyId}", request.Id);
                return null;
            }

            var oldValues = new { party.IsActive, party.EndedAt };
            if (request.Dto.IsActive.HasValue) party.IsActive = request.Dto.IsActive.Value;
            if (request.Dto.EndedAt.HasValue) party.EndedAt = request.Dto.EndedAt.Value;

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

            _logger.LogInformation("Watch party updated. Id: {PartyId}", party.Id);

            var result = await _unitOfWork.WatchParties.FindAsync(
                p => p.Id == party.Id,
                include: q => q.Include(p => p.HostProfile)
                               .Include(p => p.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show)
                               .Include(p => p.WatchPartyParticipants),
                cancellationToken: cancellationToken);

            return _mapper.Map<WatchPartyDto>(result.First());
        }
    }
}

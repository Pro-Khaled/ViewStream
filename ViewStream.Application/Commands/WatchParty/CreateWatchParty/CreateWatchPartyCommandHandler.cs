using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchParty.CreateWatchParty
{
    using WatchParty = ViewStream.Domain.Entities.WatchParty;
    public class CreateWatchPartyCommandHandler : IRequestHandler<CreateWatchPartyCommand, WatchPartyDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateWatchPartyCommandHandler> _logger;

        public CreateWatchPartyCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateWatchPartyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<WatchPartyDto> Handle(CreateWatchPartyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating watch party for EpisodeId: {EpisodeId} by ProfileId: {ProfileId}",
                request.Dto.EpisodeId, request.HostProfileId);

            var party = new WatchParty
            {
                HostProfileId = request.HostProfileId,
                EpisodeId = request.Dto.EpisodeId,
                PartyCode = GeneratePartyCode(),
                StartedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.WatchParties.AddAsync(party, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<WatchParty, object>(
                tableName: "WatchParties",
                recordId: party.Id,
                action: "INSERT",
                oldValues: null,
                newValues: new { party.HostProfileId, party.EpisodeId, party.PartyCode, party.IsActive },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Watch party created with Id: {PartyId}", party.Id);

            var result = await _unitOfWork.WatchParties.FindAsync(
                p => p.Id == party.Id,
                include: q => q.Include(p => p.HostProfile)
                               .Include(p => p.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show)
                               .Include(p => p.WatchPartyParticipants),
                cancellationToken: cancellationToken);

            return _mapper.Map<WatchPartyDto>(result.First());
        }

        private string GeneratePartyCode() => Guid.NewGuid().ToString("N")[..6].ToUpper();
    }
}

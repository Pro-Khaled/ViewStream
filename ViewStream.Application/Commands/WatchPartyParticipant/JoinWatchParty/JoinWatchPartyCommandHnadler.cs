using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchPartyParticipant.JoinWatchParty
{
    using WatchPartyParticipant = ViewStream.Domain.Entities.WatchPartyParticipant;
    public class JoinWatchPartyCommandHandler : IRequestHandler<JoinWatchPartyCommand, WatchPartyParticipantDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly IBlockCheckService _blockCheckService;
        private readonly IAvailabilityCache _availabilityCache;
        private readonly ILogger<JoinWatchPartyCommandHandler> _logger;

        public JoinWatchPartyCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            IBlockCheckService blockCheckService,
            IAvailabilityCache availabilityCache,
            ILogger<JoinWatchPartyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _blockCheckService = blockCheckService;
            _availabilityCache = availabilityCache;
            _logger = logger;
        }

        public async Task<WatchPartyParticipantDto> Handle(JoinWatchPartyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Profile {ProfileId} joining watch party {PartyId}", request.ProfileId, request.PartyId);

            var joiningProfile = await _unitOfWork.Profiles.GetByIdAsync<long>(request.ProfileId, cancellationToken);
            if (joiningProfile == null)
                throw new InvalidOperationException("Profile not found.");

            var joiningUserId = joiningProfile.UserId;

            var party = await _unitOfWork.WatchParties.GetByIdAsync<long>(request.PartyId, cancellationToken);
            if (party == null || party.IsActive != true)
                throw new InvalidOperationException("Watch party not found or inactive.");

            // ═══ ENTITLEMENT CHECK: Verify subscription tier + geo-availability ═══
            await VerifyEntitlementAsync(joiningUserId, party.EpisodeId, cancellationToken);

            // Check if any existing active participant has blocked the joining user (or vice versa)
            var currentParticipants = await _unitOfWork.WatchPartyParticipants.FindAsync(
                p => p.PartyId == request.PartyId && p.LeftAt == null,
                include: q => q.Include(p => p.Profile),
                cancellationToken: cancellationToken);

            foreach (var participantEntry in currentParticipants)
            {
                if (participantEntry.Profile != null)
                {
                    var otherUserId = participantEntry.Profile.UserId;
                    if (otherUserId == joiningUserId) continue;

                    var isBlockedByJoining = await _blockCheckService.IsBlockedAsync(joiningUserId, otherUserId);
                    var isBlockedByOther = await _blockCheckService.IsBlockedAsync(otherUserId, joiningUserId);

                    if (isBlockedByJoining || isBlockedByOther)
                    {
                        throw new InvalidOperationException("Cannot join watch party due to social block restrictions.");
                    }
                }
            }

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

        /// <summary>
        /// Verifies the user has an active subscription covering the episode and that the episode
        /// is available in their country (geo-check).
        /// </summary>
        private async Task VerifyEntitlementAsync(long userId, long episodeId, CancellationToken cancellationToken)
        {
            // 1. Load the episode with its show
            var episodes = await _unitOfWork.Episodes.FindAsync(
                e => e.Id == episodeId && e.IsDeleted != true,
                include: q => q.Include(e => e.Season).ThenInclude(s => s.Show),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var episode = episodes.FirstOrDefault();
            if (episode == null)
                throw new UnauthorizedAccessException("The episode for this watch party is no longer available.");

            // 2. Check subscription tier
            var subscriptions = await _unitOfWork.Subscriptions.FindAsync(
                s => s.UserId == userId && s.Status == "active",
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var currentSub = subscriptions.OrderByDescending(s => s.CreatedAt).FirstOrDefault();
            if (currentSub == null)
            {
                throw new UnauthorizedAccessException(
                    "You need an active subscription to join this watch party.");
            }

            // 3. Geo-availability check — resolve user's country from their profile
            var user = await _unitOfWork.Users.GetByIdAsync<long>(userId, cancellationToken);
            var userCountryCode = user?.CountryCode;

            if (!string.IsNullOrEmpty(userCountryCode))
            {
                var showId = episode.Season.ShowId;
                var availability = await _availabilityCache.GetAsync(showId, userCountryCode);

                if (availability == null)
                {
                    var availabilities = await _unitOfWork.ShowAvailabilities.FindAsync(
                        sa => sa.ShowId == showId && sa.CountryCode == userCountryCode,
                        asNoTracking: true,
                        cancellationToken: cancellationToken);
                    availability = availabilities.FirstOrDefault();

                    if (availability != null)
                        await _availabilityCache.SetAsync(showId, userCountryCode, availability);
                }

                if (availability == null)
                {
                    throw new UnauthorizedAccessException(
                        $"This content is not available in your region ({userCountryCode}). You cannot join this watch party.");
                }

                // Date-range enforcement
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                if (availability.AvailableFrom.HasValue && today < availability.AvailableFrom.Value)
                    throw new UnauthorizedAccessException("This content is not yet available in your region.");
                if (availability.AvailableUntil.HasValue && today > availability.AvailableUntil.Value)
                    throw new UnauthorizedAccessException("This content is no longer available in your region.");
            }

            _logger.LogDebug("Entitlement verified for UserId {UserId} on EpisodeId {EpisodeId}", userId, episodeId);
        }
    }
}

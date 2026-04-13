using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchPartyParticipant.JoinWatchParty
{
    using WatchPartyParticipant = ViewStream.Domain.Entities.WatchPartyParticipant;
    public class JoinWatchPartyCommandHandler : IRequestHandler<JoinWatchPartyCommand, WatchPartyParticipantDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public JoinWatchPartyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WatchPartyParticipantDto> Handle(JoinWatchPartyCommand request, CancellationToken cancellationToken)
        {
            var party = await _unitOfWork.WatchParties.GetByIdAsync<long>(request.PartyId, cancellationToken);
            if (party == null || party.IsActive != true)
                throw new InvalidOperationException("Watch party not found or inactive.");

            var existing = await _unitOfWork.WatchPartyParticipants.FindAsync(
                p => p.PartyId == request.PartyId && p.ProfileId == request.ProfileId,
                cancellationToken: cancellationToken);

            var participant = existing.FirstOrDefault();
            if (participant == null)
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
                participant.LeftAt = null;
                _unitOfWork.WatchPartyParticipants.Update(participant);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.WatchPartyParticipants.FindAsync(
                p => p.PartyId == participant.PartyId && p.ProfileId == participant.ProfileId,
                include: q => q.Include(p => p.Profile),
                cancellationToken: cancellationToken);

            return _mapper.Map<WatchPartyParticipantDto>(result.First());
        }
    }
}

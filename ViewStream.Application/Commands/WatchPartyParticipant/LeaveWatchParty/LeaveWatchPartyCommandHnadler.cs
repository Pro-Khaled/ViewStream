using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchPartyParticipant.LeaveWatchParty
{
    public class LeaveWatchPartyCommandHandler : IRequestHandler<LeaveWatchPartyCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaveWatchPartyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(LeaveWatchPartyCommand request, CancellationToken cancellationToken)
        {
            var participants = await _unitOfWork.WatchPartyParticipants.FindAsync(
                p => p.PartyId == request.PartyId && p.ProfileId == request.ProfileId,
                cancellationToken: cancellationToken);

            var participant = participants.FirstOrDefault();
            if (participant == null) return false;

            participant.LeftAt = DateTime.UtcNow;
            _unitOfWork.WatchPartyParticipants.Update(participant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

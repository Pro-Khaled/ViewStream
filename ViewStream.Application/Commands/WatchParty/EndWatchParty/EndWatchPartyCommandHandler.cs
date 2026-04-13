using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchParty.DeleteWatchParty
{
    public class EndWatchPartyCommandHandler : IRequestHandler<EndWatchPartyCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EndWatchPartyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(EndWatchPartyCommand request, CancellationToken cancellationToken)
        {
            var party = await _unitOfWork.WatchParties.GetByIdAsync<long>(request.Id, cancellationToken);
            if (party == null || party.HostProfileId != request.ProfileId)
                return false;

            party.IsActive = false;
            party.EndedAt = DateTime.UtcNow;

            _unitOfWork.WatchParties.Update(party);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

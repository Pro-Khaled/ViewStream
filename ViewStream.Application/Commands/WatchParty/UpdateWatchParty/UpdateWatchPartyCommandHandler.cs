using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchParty.UpdateWatchParty
{
    public class UpdateWatchPartyCommandHandler : IRequestHandler<UpdateWatchPartyCommand, WatchPartyDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateWatchPartyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WatchPartyDto?> Handle(UpdateWatchPartyCommand request, CancellationToken cancellationToken)
        {
            var party = await _unitOfWork.WatchParties.GetByIdAsync<long>(request.Id, cancellationToken);
            if (party == null || party.HostProfileId != request.ProfileId)
                return null;

            if (request.Dto.IsActive.HasValue) party.IsActive = request.Dto.IsActive;
            if (request.Dto.EndedAt.HasValue) party.EndedAt = request.Dto.EndedAt;

            _unitOfWork.WatchParties.Update(party);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

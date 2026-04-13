using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchParty.CreateWatchParty
{
    using WatchParty = ViewStream.Domain.Entities.WatchParty;
    public class CreateWatchPartyCommandHandler : IRequestHandler<CreateWatchPartyCommand, WatchPartyDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateWatchPartyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WatchPartyDto> Handle(CreateWatchPartyCommand request, CancellationToken cancellationToken)
        {
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

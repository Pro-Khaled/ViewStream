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

namespace ViewStream.Application.Queries.WatchParty
{
    public class GetWatchPartyByCodeQueryHandler : IRequestHandler<GetWatchPartyByCodeQuery, WatchPartyDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetWatchPartyByCodeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WatchPartyDto?> Handle(GetWatchPartyByCodeQuery request, CancellationToken cancellationToken)
        {
            var parties = await _unitOfWork.WatchParties.FindAsync(
                p => p.PartyCode == request.PartyCode && p.IsActive == true,
                include: q => q.Include(p => p.HostProfile)
                               .Include(p => p.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show)
                               .Include(p => p.WatchPartyParticipants),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<WatchPartyDto>(parties.FirstOrDefault());
        }
    }
}

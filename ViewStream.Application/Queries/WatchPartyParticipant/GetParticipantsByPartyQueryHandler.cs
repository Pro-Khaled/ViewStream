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

namespace ViewStream.Application.Queries.WatchPartyParticipant
{
    public class GetParticipantsByPartyQueryHandler : IRequestHandler<GetParticipantsByPartyQuery, List<WatchPartyParticipantDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetParticipantsByPartyQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<WatchPartyParticipantDto>> Handle(GetParticipantsByPartyQuery request, CancellationToken cancellationToken)
        {
            var participants = await _unitOfWork.WatchPartyParticipants.FindAsync(
                p => p.PartyId == request.PartyId,
                include: q => q.Include(p => p.Profile),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<List<WatchPartyParticipantDto>>(participants.OrderBy(p => p.JoinedAt));
        }
    }
}

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

namespace ViewStream.Application.Queries.PersonAward
{
    public class GetPersonAwardsQueryHandler : IRequestHandler<GetPersonAwardsQuery, List<PersonAwardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetPersonAwardsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<PersonAwardDto>> Handle(GetPersonAwardsQuery request, CancellationToken cancellationToken)
        {
            var awards = await _unitOfWork.PersonAwards.FindAsync(
                pa => pa.PersonId == request.PersonId,
                include: q => q.Include(pa => pa.Award).Include(pa => pa.Person),
                asNoTracking: true, cancellationToken: cancellationToken);
            return _mapper.Map<List<PersonAwardDto>>(awards.OrderByDescending(a => a.Award.Year));
        }
    }
}

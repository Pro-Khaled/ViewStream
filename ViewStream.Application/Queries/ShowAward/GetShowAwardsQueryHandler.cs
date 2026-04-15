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

namespace ViewStream.Application.Queries.ShowAward
{
    public class GetShowAwardsQueryHandler : IRequestHandler<GetShowAwardsQuery, List<ShowAwardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetShowAwardsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<ShowAwardDto>> Handle(GetShowAwardsQuery request, CancellationToken cancellationToken)
        {
            var awards = await _unitOfWork.ShowAwards.FindAsync(
                sa => sa.ShowId == request.ShowId,
                include: q => q.Include(sa => sa.Award).Include(sa => sa.Show),
                asNoTracking: true, cancellationToken: cancellationToken);
            return _mapper.Map<List<ShowAwardDto>>(awards.OrderByDescending(a => a.Award.Year));
        }
    }
}

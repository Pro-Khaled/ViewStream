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

namespace ViewStream.Application.Queries.Credit
{
    public class GetCreditsByPersonQueryHandler : IRequestHandler<GetCreditsByPersonQuery, List<CreditListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetCreditsByPersonQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<CreditListItemDto>> Handle(GetCreditsByPersonQuery request, CancellationToken cancellationToken)
        {
            var credits = await _unitOfWork.Credits.FindAsync(
                c => c.PersonId == request.PersonId,
                include: q => q.Include(c => c.Person)
                               .Include(c => c.Show)
                               .Include(c => c.Season).ThenInclude(s => s.Show)
                               .Include(c => c.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show),
                asNoTracking: true, cancellationToken: cancellationToken);
            return _mapper.Map<List<CreditListItemDto>>(credits.OrderBy(c => c.Show != null ? c.Show.Title : c.Season != null ? c.Season.Show.Title : c.Episode.Season.Show.Title));
        }
    }
}

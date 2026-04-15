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
    public class GetCreditsByShowQueryHandler : IRequestHandler<GetCreditsByShowQuery, List<CreditListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetCreditsByShowQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<CreditListItemDto>> Handle(GetCreditsByShowQuery request, CancellationToken cancellationToken)
        {
            var credits = await _unitOfWork.Credits.FindAsync(
                c => c.ShowId == request.ShowId,
                include: q => q.Include(c => c.Person).Include(c => c.Show),
                asNoTracking: true, cancellationToken: cancellationToken);
            return _mapper.Map<List<CreditListItemDto>>(credits.OrderBy(c => c.Role).ThenBy(c => c.Person.Name));
        }
    }
}

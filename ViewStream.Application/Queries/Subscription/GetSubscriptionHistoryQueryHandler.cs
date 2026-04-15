using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Subscription
{
    public class GetSubscriptionHistoryQueryHandler : IRequestHandler<GetSubscriptionHistoryQuery, List<SubscriptionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetSubscriptionHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<SubscriptionDto>> Handle(GetSubscriptionHistoryQuery request, CancellationToken cancellationToken)
        {
            var subs = await _unitOfWork.Subscriptions.FindAsync(
                s => s.UserId == request.UserId,
                asNoTracking: true, cancellationToken: cancellationToken);
            return _mapper.Map<List<SubscriptionDto>>(subs.OrderByDescending(s => s.CreatedAt));
        }
    }
}

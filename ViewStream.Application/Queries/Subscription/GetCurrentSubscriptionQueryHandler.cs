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
    public class GetCurrentSubscriptionQueryHandler : IRequestHandler<GetCurrentSubscriptionQuery, SubscriptionDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetCurrentSubscriptionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<SubscriptionDto?> Handle(GetCurrentSubscriptionQuery request, CancellationToken cancellationToken)
        {
            var subs = await _unitOfWork.Subscriptions.FindAsync(
                s => s.UserId == request.UserId && s.Status == "active",
                asNoTracking: true, cancellationToken: cancellationToken);
            var sub = subs.FirstOrDefault();
            return sub == null ? null : _mapper.Map<SubscriptionDto>(sub);
        }
    }
}

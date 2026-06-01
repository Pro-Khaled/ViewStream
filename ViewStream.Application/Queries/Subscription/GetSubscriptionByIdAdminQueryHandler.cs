using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Subscription
{
    public class GetSubscriptionByIdAdminQueryHandler : IRequestHandler<GetSubscriptionByIdAdminQuery, AdminSubscriptionListItemDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSubscriptionByIdAdminQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AdminSubscriptionListItemDto?> Handle(GetSubscriptionByIdAdminQuery request, CancellationToken cancellationToken)
        {
            var subscription = await _unitOfWork.Subscriptions.GetQueryable()
                .AsNoTracking()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            return subscription == null ? null : _mapper.Map<AdminSubscriptionListItemDto>(subscription);
        }
    }
}

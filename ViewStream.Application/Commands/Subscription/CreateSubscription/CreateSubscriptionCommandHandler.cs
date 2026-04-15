using AutoMapper;
using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subscription.CreateSubscription
{
    using Subscription = ViewStream.Domain.Entities.Subscription;
    public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, SubscriptionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateSubscriptionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<SubscriptionDto> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var sub = _mapper.Map<Subscription>(request.Dto);
            sub.UserId = request.UserId;
            sub.Status = "active";
            sub.StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
            sub.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Subscriptions.AddAsync(sub, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<SubscriptionDto>(sub);
        }
    }
}

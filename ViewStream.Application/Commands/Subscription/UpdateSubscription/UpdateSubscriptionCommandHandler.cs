using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;


namespace ViewStream.Application.Commands.Subscription.UpdateSubscription
{
    public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, SubscriptionDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateSubscriptionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<SubscriptionDto?> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var sub = await _unitOfWork.Subscriptions.GetByIdAsync<long>(request.Id, cancellationToken);
            if (sub == null) return null;
            _mapper.Map(request.Dto, sub);
            _unitOfWork.Subscriptions.Update(sub);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<SubscriptionDto>(sub);
        }
    }
}

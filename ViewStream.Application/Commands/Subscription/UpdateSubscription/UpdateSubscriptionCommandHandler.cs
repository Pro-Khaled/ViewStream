using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subscription.UpdateSubscription
{
//    public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, BaseResponse<SubscriptionDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateSubscriptionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SubscriptionDto>> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Subscriptions.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SubscriptionDto>.Fail("Subscription not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Subscriptions.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<SubscriptionDto>(entity);
//                // return BaseResponse<SubscriptionDto>.Ok(dto, "Subscription updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SubscriptionDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}

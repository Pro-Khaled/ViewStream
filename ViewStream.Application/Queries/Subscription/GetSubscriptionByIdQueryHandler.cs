using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Subscription
{
//    public class GetSubscriptionByIdQueryHandler : IRequestHandler<GetSubscriptionByIdQuery, BaseResponse<SubscriptionDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetSubscriptionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SubscriptionDto>> Handle(GetSubscriptionByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Subscriptions.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SubscriptionDto>.Fail("Subscription not found");
//                
//                var dto = _mapper.Map<SubscriptionDto>(entity);
//                return BaseResponse<SubscriptionDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SubscriptionDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

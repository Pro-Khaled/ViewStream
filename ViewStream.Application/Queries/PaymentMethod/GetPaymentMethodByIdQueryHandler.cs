using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PaymentMethod
{
//    public class GetPaymentMethodByIdQueryHandler : IRequestHandler<GetPaymentMethodByIdQuery, BaseResponse<PaymentMethodDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetPaymentMethodByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PaymentMethodDto>> Handle(GetPaymentMethodByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.PaymentMethods.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PaymentMethodDto>.Fail("PaymentMethod not found");
//                
//                var dto = _mapper.Map<PaymentMethodDto>(entity);
//                return BaseResponse<PaymentMethodDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PaymentMethodDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

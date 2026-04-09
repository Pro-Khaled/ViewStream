using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PaymentMethod
{
//    public class GetAllPaymentMethodsQueryHandler : IRequestHandler<GetAllPaymentMethodsQuery, BaseResponse<PagedResult<PaymentMethodDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAllPaymentMethodsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PagedResult<PaymentMethodDto>>> Handle(GetAllPaymentMethodsQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entities = await _unitOfWork.PaymentMethods.GetAllAsync();
//                var entityList = entities.ToList();
//                
//                // TODO: Apply search, sort, pagination
//                
//                var dtos = _mapper.Map<List<PaymentMethodDto>>(entityList);
//                var result = new PagedResult<PaymentMethodDto>
//                {
//                    Items = dtos,
//                    TotalCount = entityList.Count,
//                    PageNumber = request.PageNumber,
//                    PageSize = request.PageSize
//                };
//                
//                return BaseResponse<PagedResult<PaymentMethodDto>>.Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PagedResult<PaymentMethodDto>>.Fail($"Error retrieving s: {ex.Message}");
//            }
//        }
//    }
}

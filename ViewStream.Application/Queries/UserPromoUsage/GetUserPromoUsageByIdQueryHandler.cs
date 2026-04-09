using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserPromoUsage
{
//    public class GetUserPromoUsageByIdQueryHandler : IRequestHandler<GetUserPromoUsageByIdQuery, BaseResponse<UserPromoUsageDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetUserPromoUsageByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserPromoUsageDto>> Handle(GetUserPromoUsageByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserPromoUsages.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserPromoUsageDto>.Fail("UserPromoUsage not found");
//                
//                var dto = _mapper.Map<UserPromoUsageDto>(entity);
//                return BaseResponse<UserPromoUsageDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserPromoUsageDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PromoCode
{
//    public class GetPromoCodeByIdQueryHandler : IRequestHandler<GetPromoCodeByIdQuery, BaseResponse<PromoCodeDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetPromoCodeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PromoCodeDto>> Handle(GetPromoCodeByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.PromoCodes.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PromoCodeDto>.Fail("PromoCode not found");
//                
//                var dto = _mapper.Map<PromoCodeDto>(entity);
//                return BaseResponse<PromoCodeDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PromoCodeDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

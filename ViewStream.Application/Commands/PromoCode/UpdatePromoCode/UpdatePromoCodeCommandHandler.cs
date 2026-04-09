using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PromoCode.UpdatePromoCode
{
//    public class UpdatePromoCodeCommandHandler : IRequestHandler<UpdatePromoCodeCommand, BaseResponse<PromoCodeDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdatePromoCodeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PromoCodeDto>> Handle(UpdatePromoCodeCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.PromoCodes.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PromoCodeDto>.Fail("PromoCode not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.PromoCodes.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<PromoCodeDto>(entity);
//                // return BaseResponse<PromoCodeDto>.Ok(dto, "PromoCode updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PromoCodeDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}

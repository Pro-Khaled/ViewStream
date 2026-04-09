using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserPromoUsage.UpdateUserPromoUsage
{
//    public class UpdateUserPromoUsageCommandHandler : IRequestHandler<UpdateUserPromoUsageCommand, BaseResponse<UserPromoUsageDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateUserPromoUsageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserPromoUsageDto>> Handle(UpdateUserPromoUsageCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserPromoUsages.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserPromoUsageDto>.Fail("UserPromoUsage not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.UserPromoUsages.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<UserPromoUsageDto>(entity);
//                // return BaseResponse<UserPromoUsageDto>.Ok(dto, "UserPromoUsage updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserPromoUsageDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}

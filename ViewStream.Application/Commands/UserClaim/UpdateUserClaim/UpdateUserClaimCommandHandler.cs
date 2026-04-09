using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserClaim.UpdateUserClaim
{
//    public class UpdateUserClaimCommandHandler : IRequestHandler<UpdateUserClaimCommand, BaseResponse<UserClaimDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateUserClaimCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserClaimDto>> Handle(UpdateUserClaimCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserClaims.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserClaimDto>.Fail("UserClaim not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.UserClaims.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<UserClaimDto>(entity);
//                // return BaseResponse<UserClaimDto>.Ok(dto, "UserClaim updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserClaimDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}

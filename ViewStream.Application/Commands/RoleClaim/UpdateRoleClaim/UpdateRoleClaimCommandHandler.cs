using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.RoleClaim.UpdateRoleClaim
{
//    public class UpdateRoleClaimCommandHandler : IRequestHandler<UpdateRoleClaimCommand, BaseResponse<RoleClaimDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateRoleClaimCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<RoleClaimDto>> Handle(UpdateRoleClaimCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.RoleClaims.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<RoleClaimDto>.Fail("RoleClaim not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.RoleClaims.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<RoleClaimDto>(entity);
//                // return BaseResponse<RoleClaimDto>.Ok(dto, "RoleClaim updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<RoleClaimDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}

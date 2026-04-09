using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Permission.UpdatePermission
{
//    public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, BaseResponse<PermissionDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdatePermissionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PermissionDto>> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Permissions.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PermissionDto>.Fail("Permission not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Permissions.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<PermissionDto>(entity);
//                // return BaseResponse<PermissionDto>.Ok(dto, "Permission updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PermissionDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}

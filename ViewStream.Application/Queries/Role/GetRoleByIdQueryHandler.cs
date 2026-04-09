using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Role
{
//    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, BaseResponse<RoleDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetRoleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Roles.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<RoleDto>.Fail("Role not found");
//                
//                var dto = _mapper.Map<RoleDto>(entity);
//                return BaseResponse<RoleDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<RoleDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

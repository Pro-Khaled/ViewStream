using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserRole
{
//    public class GetUserRoleByIdQueryHandler : IRequestHandler<GetUserRoleByIdQuery, BaseResponse<UserRoleDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetUserRoleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserRoleDto>> Handle(GetUserRoleByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserRoles.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserRoleDto>.Fail("UserRole not found");
//                
//                var dto = _mapper.Map<UserRoleDto>(entity);
//                return BaseResponse<UserRoleDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserRoleDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

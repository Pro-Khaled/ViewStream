using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Permission
{
//    public class GetPermissionByIdQueryHandler : IRequestHandler<GetPermissionByIdQuery, BaseResponse<PermissionDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetPermissionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PermissionDto>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Permissions.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PermissionDto>.Fail("Permission not found");
//                
//                var dto = _mapper.Map<PermissionDto>(entity);
//                return BaseResponse<PermissionDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PermissionDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

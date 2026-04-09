using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.RoleClaim
{
//    public class GetRoleClaimByIdQueryHandler : IRequestHandler<GetRoleClaimByIdQuery, BaseResponse<RoleClaimDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetRoleClaimByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<RoleClaimDto>> Handle(GetRoleClaimByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.RoleClaims.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<RoleClaimDto>.Fail("RoleClaim not found");
//                
//                var dto = _mapper.Map<RoleClaimDto>(entity);
//                return BaseResponse<RoleClaimDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<RoleClaimDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

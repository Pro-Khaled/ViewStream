using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserClaim
{
//    public class GetUserClaimByIdQueryHandler : IRequestHandler<GetUserClaimByIdQuery, BaseResponse<UserClaimDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetUserClaimByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<UserClaimDto>> Handle(GetUserClaimByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.UserClaims.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<UserClaimDto>.Fail("UserClaim not found");
//                
//                var dto = _mapper.Map<UserClaimDto>(entity);
//                return BaseResponse<UserClaimDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<UserClaimDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

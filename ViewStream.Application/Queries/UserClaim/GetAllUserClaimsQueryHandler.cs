using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserClaim
{
//    public class GetAllUserClaimsQueryHandler : IRequestHandler<GetAllUserClaimsQuery, BaseResponse<PagedResult<UserClaimDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAllUserClaimsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PagedResult<UserClaimDto>>> Handle(GetAllUserClaimsQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entities = await _unitOfWork.UserClaims.GetAllAsync();
//                var entityList = entities.ToList();
//                
//                // TODO: Apply search, sort, pagination
//                
//                var dtos = _mapper.Map<List<UserClaimDto>>(entityList);
//                var result = new PagedResult<UserClaimDto>
//                {
//                    Items = dtos,
//                    TotalCount = entityList.Count,
//                    PageNumber = request.PageNumber,
//                    PageSize = request.PageSize
//                };
//                
//                return BaseResponse<PagedResult<UserClaimDto>>.Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PagedResult<UserClaimDto>>.Fail($"Error retrieving s: {ex.Message}");
//            }
//        }
//    }
}

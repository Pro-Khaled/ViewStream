using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserPromoUsage
{
//    public class GetAllUserPromoUsagesQueryHandler : IRequestHandler<GetAllUserPromoUsagesQuery, BaseResponse<PagedResult<UserPromoUsageDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAllUserPromoUsagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PagedResult<UserPromoUsageDto>>> Handle(GetAllUserPromoUsagesQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entities = await _unitOfWork.UserPromoUsages.GetAllAsync();
//                var entityList = entities.ToList();
//                
//                // TODO: Apply search, sort, pagination
//                
//                var dtos = _mapper.Map<List<UserPromoUsageDto>>(entityList);
//                var result = new PagedResult<UserPromoUsageDto>
//                {
//                    Items = dtos,
//                    TotalCount = entityList.Count,
//                    PageNumber = request.PageNumber,
//                    PageSize = request.PageSize
//                };
//                
//                return BaseResponse<PagedResult<UserPromoUsageDto>>.Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PagedResult<UserPromoUsageDto>>.Fail($"Error retrieving s: {ex.Message}");
//            }
//        }
//    }
}

using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.WatchHistory
{
//    public class GetAllWatchHistorysQueryHandler : IRequestHandler<GetAllWatchHistorysQuery, BaseResponse<PagedResult<WatchHistoryDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAllWatchHistorysQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PagedResult<WatchHistoryDto>>> Handle(GetAllWatchHistorysQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entities = await _unitOfWork.WatchHistorys.GetAllAsync();
//                var entityList = entities.ToList();
//                
//                // TODO: Apply search, sort, pagination
//                
//                var dtos = _mapper.Map<List<WatchHistoryDto>>(entityList);
//                var result = new PagedResult<WatchHistoryDto>
//                {
//                    Items = dtos,
//                    TotalCount = entityList.Count,
//                    PageNumber = request.PageNumber,
//                    PageSize = request.PageSize
//                };
//                
//                return BaseResponse<PagedResult<WatchHistoryDto>>.Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PagedResult<WatchHistoryDto>>.Fail($"Error retrieving s: {ex.Message}");
//            }
//        }
//    }
}

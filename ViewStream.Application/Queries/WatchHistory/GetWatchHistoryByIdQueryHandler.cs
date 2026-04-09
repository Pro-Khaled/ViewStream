using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.WatchHistory
{
//    public class GetWatchHistoryByIdQueryHandler : IRequestHandler<GetWatchHistoryByIdQuery, BaseResponse<WatchHistoryDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetWatchHistoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<WatchHistoryDto>> Handle(GetWatchHistoryByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.WatchHistorys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<WatchHistoryDto>.Fail("WatchHistory not found");
//                
//                var dto = _mapper.Map<WatchHistoryDto>(entity);
//                return BaseResponse<WatchHistoryDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<WatchHistoryDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

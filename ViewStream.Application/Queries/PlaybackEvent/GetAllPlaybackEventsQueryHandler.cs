using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PlaybackEvent
{
//    public class GetAllPlaybackEventsQueryHandler : IRequestHandler<GetAllPlaybackEventsQuery, BaseResponse<PagedResult<PlaybackEventDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAllPlaybackEventsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PagedResult<PlaybackEventDto>>> Handle(GetAllPlaybackEventsQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entities = await _unitOfWork.PlaybackEvents.GetAllAsync();
//                var entityList = entities.ToList();
//                
//                // TODO: Apply search, sort, pagination
//                
//                var dtos = _mapper.Map<List<PlaybackEventDto>>(entityList);
//                var result = new PagedResult<PlaybackEventDto>
//                {
//                    Items = dtos,
//                    TotalCount = entityList.Count,
//                    PageNumber = request.PageNumber,
//                    PageSize = request.PageSize
//                };
//                
//                return BaseResponse<PagedResult<PlaybackEventDto>>.Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PagedResult<PlaybackEventDto>>.Fail($"Error retrieving s: {ex.Message}");
//            }
//        }
//    }
}

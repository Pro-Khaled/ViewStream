using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PlaybackEvent
{
//    public class GetPlaybackEventByIdQueryHandler : IRequestHandler<GetPlaybackEventByIdQuery, BaseResponse<PlaybackEventDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetPlaybackEventByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PlaybackEventDto>> Handle(GetPlaybackEventByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.PlaybackEvents.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PlaybackEventDto>.Fail("PlaybackEvent not found");
//                
//                var dto = _mapper.Map<PlaybackEventDto>(entity);
//                return BaseResponse<PlaybackEventDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PlaybackEventDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

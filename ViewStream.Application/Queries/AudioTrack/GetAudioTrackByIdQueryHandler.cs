using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.AudioTrack
{
//    public class GetAudioTrackByIdQueryHandler : IRequestHandler<GetAudioTrackByIdQuery, BaseResponse<AudioTrackDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetAudioTrackByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<AudioTrackDto>> Handle(GetAudioTrackByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.AudioTracks.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<AudioTrackDto>.Fail("AudioTrack not found");
//                
//                var dto = _mapper.Map<AudioTrackDto>(entity);
//                return BaseResponse<AudioTrackDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<AudioTrackDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

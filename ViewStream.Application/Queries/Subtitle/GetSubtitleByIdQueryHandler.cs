using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Subtitle
{
//    public class GetSubtitleByIdQueryHandler : IRequestHandler<GetSubtitleByIdQuery, BaseResponse<SubtitleDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetSubtitleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<SubtitleDto>> Handle(GetSubtitleByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Subtitles.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<SubtitleDto>.Fail("Subtitle not found");
//                
//                var dto = _mapper.Map<SubtitleDto>(entity);
//                return BaseResponse<SubtitleDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<SubtitleDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

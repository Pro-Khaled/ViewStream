using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.EpisodeComment
{
//    public class GetEpisodeCommentByIdQueryHandler : IRequestHandler<GetEpisodeCommentByIdQuery, BaseResponse<EpisodeCommentDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetEpisodeCommentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<EpisodeCommentDto>> Handle(GetEpisodeCommentByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.EpisodeComments.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<EpisodeCommentDto>.Fail("EpisodeComment not found");
//                
//                var dto = _mapper.Map<EpisodeCommentDto>(entity);
//                return BaseResponse<EpisodeCommentDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<EpisodeCommentDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

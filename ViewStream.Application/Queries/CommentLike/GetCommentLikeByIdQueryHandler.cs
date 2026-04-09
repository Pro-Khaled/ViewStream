using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.CommentLike
{
//    public class GetCommentLikeByIdQueryHandler : IRequestHandler<GetCommentLikeByIdQuery, BaseResponse<CommentLikeDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetCommentLikeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<CommentLikeDto>> Handle(GetCommentLikeByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.CommentLikes.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<CommentLikeDto>.Fail("CommentLike not found");
//                
//                var dto = _mapper.Map<CommentLikeDto>(entity);
//                return BaseResponse<CommentLikeDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<CommentLikeDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}

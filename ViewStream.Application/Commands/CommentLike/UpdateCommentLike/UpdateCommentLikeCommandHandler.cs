using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentLike.UpdateCommentLike
{
//    public class UpdateCommentLikeCommandHandler : IRequestHandler<UpdateCommentLikeCommand, BaseResponse<CommentLikeDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateCommentLikeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<CommentLikeDto>> Handle(UpdateCommentLikeCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.CommentLikes.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<CommentLikeDto>.Fail("CommentLike not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.CommentLikes.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<CommentLikeDto>(entity);
//                // return BaseResponse<CommentLikeDto>.Ok(dto, "CommentLike updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<CommentLikeDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}

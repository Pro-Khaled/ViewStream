using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EpisodeComment.UpdateEpisodeComment
{
//    public class UpdateEpisodeCommentCommandHandler : IRequestHandler<UpdateEpisodeCommentCommand, BaseResponse<EpisodeCommentDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateEpisodeCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<EpisodeCommentDto>> Handle(UpdateEpisodeCommentCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.EpisodeComments.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<EpisodeCommentDto>.Fail("EpisodeComment not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.EpisodeComments.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<EpisodeCommentDto>(entity);
//                // return BaseResponse<EpisodeCommentDto>.Ok(dto, "EpisodeComment updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<EpisodeCommentDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}

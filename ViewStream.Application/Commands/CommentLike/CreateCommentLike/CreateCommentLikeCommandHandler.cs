using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentLike.CreateCommentLike
{
  //  public class CreateCommentLikeCommandHandler : IRequestHandler<CreateCommentLikeCommand, BaseResponse<CommentLikeDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateCommentLikeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<CommentLikeDto>> Handle(CreateCommentLikeCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<CommentLike>(request);
  //              
  //              // await _unitOfWork.CommentLikes.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<CommentLikeDto>(entity);
  //              // return BaseResponse<CommentLikeDto>.Ok(dto, "CommentLike created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<CommentLikeDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

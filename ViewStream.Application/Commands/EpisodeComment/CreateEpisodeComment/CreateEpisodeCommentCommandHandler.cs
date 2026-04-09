using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EpisodeComment.CreateEpisodeComment
{
  //  public class CreateEpisodeCommentCommandHandler : IRequestHandler<CreateEpisodeCommentCommand, BaseResponse<EpisodeCommentDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateEpisodeCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<EpisodeCommentDto>> Handle(CreateEpisodeCommentCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<EpisodeComment>(request);
  //              
  //              // await _unitOfWork.EpisodeComments.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<EpisodeCommentDto>(entity);
  //              // return BaseResponse<EpisodeCommentDto>.Ok(dto, "EpisodeComment created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<EpisodeCommentDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.CreateSubtitle
{
  //  public class CreateSubtitleCommandHandler : IRequestHandler<CreateSubtitleCommand, BaseResponse<SubtitleDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateSubtitleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<SubtitleDto>> Handle(CreateSubtitleCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Subtitle>(request);
  //              
  //              // await _unitOfWork.Subtitles.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<SubtitleDto>(entity);
  //              // return BaseResponse<SubtitleDto>.Ok(dto, "Subtitle created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<SubtitleDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

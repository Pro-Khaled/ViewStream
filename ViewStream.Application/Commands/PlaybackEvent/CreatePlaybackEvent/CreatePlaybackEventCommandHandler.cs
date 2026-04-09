using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PlaybackEvent.CreatePlaybackEvent
{
  //  public class CreatePlaybackEventCommandHandler : IRequestHandler<CreatePlaybackEventCommand, BaseResponse<PlaybackEventDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreatePlaybackEventCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<PlaybackEventDto>> Handle(CreatePlaybackEventCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<PlaybackEvent>(request);
  //              
  //              // await _unitOfWork.PlaybackEvents.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<PlaybackEventDto>(entity);
  //              // return BaseResponse<PlaybackEventDto>.Ok(dto, "PlaybackEvent created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<PlaybackEventDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

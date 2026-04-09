using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AudioTrack.CreateAudioTrack
{
  //  public class CreateAudioTrackCommandHandler : IRequestHandler<CreateAudioTrackCommand, BaseResponse<AudioTrackDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateAudioTrackCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<AudioTrackDto>> Handle(CreateAudioTrackCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<AudioTrack>(request);
  //              
  //              // await _unitOfWork.AudioTracks.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<AudioTrackDto>(entity);
  //              // return BaseResponse<AudioTrackDto>.Ok(dto, "AudioTrack created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<AudioTrackDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

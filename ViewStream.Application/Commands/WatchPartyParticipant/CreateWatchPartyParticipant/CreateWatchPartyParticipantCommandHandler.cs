using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchPartyParticipant.CreateWatchPartyParticipant
{
  //  public class CreateWatchPartyParticipantCommandHandler : IRequestHandler<CreateWatchPartyParticipantCommand, BaseResponse<WatchPartyParticipantDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateWatchPartyParticipantCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<WatchPartyParticipantDto>> Handle(CreateWatchPartyParticipantCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<WatchPartyParticipant>(request);
  //              
  //              // await _unitOfWork.WatchPartyParticipants.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<WatchPartyParticipantDto>(entity);
  //              // return BaseResponse<WatchPartyParticipantDto>.Ok(dto, "WatchPartyParticipant created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<WatchPartyParticipantDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

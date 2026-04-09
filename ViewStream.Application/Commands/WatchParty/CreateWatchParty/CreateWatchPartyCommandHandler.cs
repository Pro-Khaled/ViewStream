using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchParty.CreateWatchParty
{
  //  public class CreateWatchPartyCommandHandler : IRequestHandler<CreateWatchPartyCommand, BaseResponse<WatchPartyDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateWatchPartyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<WatchPartyDto>> Handle(CreateWatchPartyCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<WatchParty>(request);
  //              
  //              // await _unitOfWork.WatchPartys.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<WatchPartyDto>(entity);
  //              // return BaseResponse<WatchPartyDto>.Ok(dto, "WatchParty created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<WatchPartyDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

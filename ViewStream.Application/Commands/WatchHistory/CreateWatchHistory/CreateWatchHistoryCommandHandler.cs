using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.WatchHistory.CreateWatchHistory
{
  //  public class CreateWatchHistoryCommandHandler : IRequestHandler<CreateWatchHistoryCommand, BaseResponse<WatchHistoryDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateWatchHistoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<WatchHistoryDto>> Handle(CreateWatchHistoryCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<WatchHistory>(request);
  //              
  //              // await _unitOfWork.WatchHistorys.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<WatchHistoryDto>(entity);
  //              // return BaseResponse<WatchHistoryDto>.Ok(dto, "WatchHistory created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<WatchHistoryDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

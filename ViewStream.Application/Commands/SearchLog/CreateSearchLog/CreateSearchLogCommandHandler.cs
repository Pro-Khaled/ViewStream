using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SearchLog.CreateSearchLog
{
  //  public class CreateSearchLogCommandHandler : IRequestHandler<CreateSearchLogCommand, BaseResponse<SearchLogDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateSearchLogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<SearchLogDto>> Handle(CreateSearchLogCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<SearchLog>(request);
  //              
  //              // await _unitOfWork.SearchLogs.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<SearchLogDto>(entity);
  //              // return BaseResponse<SearchLogDto>.Ok(dto, "SearchLog created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<SearchLogDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

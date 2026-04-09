using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ErrorLog.CreateErrorLog
{
  //  public class CreateErrorLogCommandHandler : IRequestHandler<CreateErrorLogCommand, BaseResponse<ErrorLogDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateErrorLogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<ErrorLogDto>> Handle(CreateErrorLogCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<ErrorLog>(request);
  //              
  //              // await _unitOfWork.ErrorLogs.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<ErrorLogDto>(entity);
  //              // return BaseResponse<ErrorLogDto>.Ok(dto, "ErrorLog created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<ErrorLogDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

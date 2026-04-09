using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AuditLog.CreateAuditLog
{
  //  public class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, BaseResponse<AuditLogDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateAuditLogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<AuditLogDto>> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<AuditLog>(request);
  //              
  //              // await _unitOfWork.AuditLogs.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<AuditLogDto>(entity);
  //              // return BaseResponse<AuditLogDto>.Ok(dto, "AuditLog created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<AuditLogDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

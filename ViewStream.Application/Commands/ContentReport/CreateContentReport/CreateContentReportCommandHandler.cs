using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentReport.CreateContentReport
{
  //  public class CreateContentReportCommandHandler : IRequestHandler<CreateContentReportCommand, BaseResponse<ContentReportDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateContentReportCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<ContentReportDto>> Handle(CreateContentReportCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<ContentReport>(request);
  //              
  //              // await _unitOfWork.ContentReports.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<ContentReportDto>(entity);
  //              // return BaseResponse<ContentReportDto>.Ok(dto, "ContentReport created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<ContentReportDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

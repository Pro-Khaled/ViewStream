using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentReport.CreateCommentReport
{
  //  public class CreateCommentReportCommandHandler : IRequestHandler<CreateCommentReportCommand, BaseResponse<CommentReportDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateCommentReportCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<CommentReportDto>> Handle(CreateCommentReportCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<CommentReport>(request);
  //              
  //              // await _unitOfWork.CommentReports.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<CommentReportDto>(entity);
  //              // return BaseResponse<CommentReportDto>.Ok(dto, "CommentReport created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<CommentReportDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

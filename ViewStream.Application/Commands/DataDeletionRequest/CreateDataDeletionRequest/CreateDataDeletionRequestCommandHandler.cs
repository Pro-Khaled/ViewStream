using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.DataDeletionRequest.CreateDataDeletionRequest
{
  //  public class CreateDataDeletionRequestCommandHandler : IRequestHandler<CreateDataDeletionRequestCommand, BaseResponse<DataDeletionRequestDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateDataDeletionRequestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<DataDeletionRequestDto>> Handle(CreateDataDeletionRequestCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<DataDeletionRequest>(request);
  //              
  //              // await _unitOfWork.DataDeletionRequests.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<DataDeletionRequestDto>(entity);
  //              // return BaseResponse<DataDeletionRequestDto>.Ok(dto, "DataDeletionRequest created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<DataDeletionRequestDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

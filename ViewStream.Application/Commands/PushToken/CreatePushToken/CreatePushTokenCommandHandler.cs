using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PushToken.CreatePushToken
{
  //  public class CreatePushTokenCommandHandler : IRequestHandler<CreatePushTokenCommand, BaseResponse<PushTokenDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreatePushTokenCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<PushTokenDto>> Handle(CreatePushTokenCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<PushToken>(request);
  //              
  //              // await _unitOfWork.PushTokens.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<PushTokenDto>(entity);
  //              // return BaseResponse<PushTokenDto>.Ok(dto, "PushToken created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<PushTokenDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

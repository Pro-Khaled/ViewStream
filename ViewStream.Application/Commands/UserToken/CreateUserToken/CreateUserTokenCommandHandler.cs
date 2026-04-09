using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserToken.CreateUserToken
{
  //  public class CreateUserTokenCommandHandler : IRequestHandler<CreateUserTokenCommand, BaseResponse<UserTokenDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateUserTokenCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<UserTokenDto>> Handle(CreateUserTokenCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<UserToken>(request);
  //              
  //              // await _unitOfWork.UserTokens.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<UserTokenDto>(entity);
  //              // return BaseResponse<UserTokenDto>.Ok(dto, "UserToken created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<UserTokenDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

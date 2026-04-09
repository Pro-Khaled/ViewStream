using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLogin.CreateUserLogin
{
  //  public class CreateUserLoginCommandHandler : IRequestHandler<CreateUserLoginCommand, BaseResponse<UserLoginDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateUserLoginCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<UserLoginDto>> Handle(CreateUserLoginCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<UserLogin>(request);
  //              
  //              // await _unitOfWork.UserLogins.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<UserLoginDto>(entity);
  //              // return BaseResponse<UserLoginDto>.Ok(dto, "UserLogin created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<UserLoginDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

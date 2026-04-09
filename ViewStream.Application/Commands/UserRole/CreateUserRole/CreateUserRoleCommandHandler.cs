using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserRole.CreateUserRole
{
  //  public class CreateUserRoleCommandHandler : IRequestHandler<CreateUserRoleCommand, BaseResponse<UserRoleDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateUserRoleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<UserRoleDto>> Handle(CreateUserRoleCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<UserRole>(request);
  //              
  //              // await _unitOfWork.UserRoles.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<UserRoleDto>(entity);
  //              // return BaseResponse<UserRoleDto>.Ok(dto, "UserRole created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<UserRoleDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Permission.CreatePermission
{
  //  public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, BaseResponse<PermissionDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreatePermissionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<PermissionDto>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Permission>(request);
  //              
  //              // await _unitOfWork.Permissions.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<PermissionDto>(entity);
  //              // return BaseResponse<PermissionDto>.Ok(dto, "Permission created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<PermissionDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

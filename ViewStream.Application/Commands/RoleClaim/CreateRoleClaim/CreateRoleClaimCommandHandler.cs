using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.RoleClaim.CreateRoleClaim
{
  //  public class CreateRoleClaimCommandHandler : IRequestHandler<CreateRoleClaimCommand, BaseResponse<RoleClaimDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateRoleClaimCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<RoleClaimDto>> Handle(CreateRoleClaimCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<RoleClaim>(request);
  //              
  //              // await _unitOfWork.RoleClaims.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<RoleClaimDto>(entity);
  //              // return BaseResponse<RoleClaimDto>.Ok(dto, "RoleClaim created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<RoleClaimDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

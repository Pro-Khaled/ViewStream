using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserClaim.CreateUserClaim
{
  //  public class CreateUserClaimCommandHandler : IRequestHandler<CreateUserClaimCommand, BaseResponse<UserClaimDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateUserClaimCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<UserClaimDto>> Handle(CreateUserClaimCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<UserClaim>(request);
  //              
  //              // await _unitOfWork.UserClaims.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<UserClaimDto>(entity);
  //              // return BaseResponse<UserClaimDto>.Ok(dto, "UserClaim created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<UserClaimDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

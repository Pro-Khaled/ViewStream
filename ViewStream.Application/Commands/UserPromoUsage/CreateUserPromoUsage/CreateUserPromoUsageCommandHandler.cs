using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserPromoUsage.CreateUserPromoUsage
{
  //  public class CreateUserPromoUsageCommandHandler : IRequestHandler<CreateUserPromoUsageCommand, BaseResponse<UserPromoUsageDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateUserPromoUsageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<UserPromoUsageDto>> Handle(CreateUserPromoUsageCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<UserPromoUsage>(request);
  //              
  //              // await _unitOfWork.UserPromoUsages.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<UserPromoUsageDto>(entity);
  //              // return BaseResponse<UserPromoUsageDto>.Ok(dto, "UserPromoUsage created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<UserPromoUsageDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

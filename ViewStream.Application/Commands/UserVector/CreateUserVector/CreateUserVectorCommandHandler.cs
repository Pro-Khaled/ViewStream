using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserVector.CreateUserVector
{
  //  public class CreateUserVectorCommandHandler : IRequestHandler<CreateUserVectorCommand, BaseResponse<UserVectorDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateUserVectorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<UserVectorDto>> Handle(CreateUserVectorCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<UserVector>(request);
  //              
  //              // await _unitOfWork.UserVectors.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<UserVectorDto>(entity);
  //              // return BaseResponse<UserVectorDto>.Ok(dto, "UserVector created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<UserVectorDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

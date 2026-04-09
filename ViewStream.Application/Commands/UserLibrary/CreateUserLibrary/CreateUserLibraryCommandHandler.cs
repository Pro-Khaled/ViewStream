using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserLibrary.CreateUserLibrary
{
  //  public class CreateUserLibraryCommandHandler : IRequestHandler<CreateUserLibraryCommand, BaseResponse<UserLibraryDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateUserLibraryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<UserLibraryDto>> Handle(CreateUserLibraryCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<UserLibrary>(request);
  //              
  //              // await _unitOfWork.UserLibrarys.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<UserLibraryDto>(entity);
  //              // return BaseResponse<UserLibraryDto>.Ok(dto, "UserLibrary created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<UserLibraryDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

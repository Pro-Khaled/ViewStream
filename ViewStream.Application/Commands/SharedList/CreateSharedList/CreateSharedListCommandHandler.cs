using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedList.CreateSharedList
{
  //  public class CreateSharedListCommandHandler : IRequestHandler<CreateSharedListCommand, BaseResponse<SharedListDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateSharedListCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<SharedListDto>> Handle(CreateSharedListCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<SharedList>(request);
  //              
  //              // await _unitOfWork.SharedLists.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<SharedListDto>(entity);
  //              // return BaseResponse<SharedListDto>.Ok(dto, "SharedList created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<SharedListDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

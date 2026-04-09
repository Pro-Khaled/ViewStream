using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SharedListItem.CreateSharedListItem
{
  //  public class CreateSharedListItemCommandHandler : IRequestHandler<CreateSharedListItemCommand, BaseResponse<SharedListItemDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateSharedListItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<SharedListItemDto>> Handle(CreateSharedListItemCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<SharedListItem>(request);
  //              
  //              // await _unitOfWork.SharedListItems.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<SharedListItemDto>(entity);
  //              // return BaseResponse<SharedListItemDto>.Ok(dto, "SharedListItem created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<SharedListItemDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

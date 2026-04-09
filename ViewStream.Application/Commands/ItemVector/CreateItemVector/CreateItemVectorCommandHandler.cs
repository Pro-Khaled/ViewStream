using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ItemVector.CreateItemVector
{
  //  public class CreateItemVectorCommandHandler : IRequestHandler<CreateItemVectorCommand, BaseResponse<ItemVectorDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateItemVectorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<ItemVectorDto>> Handle(CreateItemVectorCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<ItemVector>(request);
  //              
  //              // await _unitOfWork.ItemVectors.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<ItemVectorDto>(entity);
  //              // return BaseResponse<ItemVectorDto>.Ok(dto, "ItemVector created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<ItemVectorDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

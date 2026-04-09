using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentTag.CreateContentTag
{
  //  public class CreateContentTagCommandHandler : IRequestHandler<CreateContentTagCommand, BaseResponse<ContentTagDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateContentTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<ContentTagDto>> Handle(CreateContentTagCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<ContentTag>(request);
  //              
  //              // await _unitOfWork.ContentTags.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<ContentTagDto>(entity);
  //              // return BaseResponse<ContentTagDto>.Ok(dto, "ContentTag created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<ContentTagDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

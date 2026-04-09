using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserInteraction.CreateUserInteraction
{
  //  public class CreateUserInteractionCommandHandler : IRequestHandler<CreateUserInteractionCommand, BaseResponse<UserInteractionDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateUserInteractionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<UserInteractionDto>> Handle(CreateUserInteractionCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<UserInteraction>(request);
  //              
  //              // await _unitOfWork.UserInteractions.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<UserInteractionDto>(entity);
  //              // return BaseResponse<UserInteractionDto>.Ok(dto, "UserInteraction created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<UserInteractionDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

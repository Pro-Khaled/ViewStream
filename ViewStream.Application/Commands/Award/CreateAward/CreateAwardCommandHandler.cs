using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Award.CreateAward
{
  //  public class CreateAwardCommandHandler : IRequestHandler<CreateAwardCommand, BaseResponse<AwardDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateAwardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<AwardDto>> Handle(CreateAwardCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Award>(request);
  //              
  //              // await _unitOfWork.Awards.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<AwardDto>(entity);
  //              // return BaseResponse<AwardDto>.Ok(dto, "Award created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<AwardDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

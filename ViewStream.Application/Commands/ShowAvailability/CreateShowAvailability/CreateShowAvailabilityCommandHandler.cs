using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAvailability.CreateShowAvailability
{
  //  public class CreateShowAvailabilityCommandHandler : IRequestHandler<CreateShowAvailabilityCommand, BaseResponse<ShowAvailabilityDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateShowAvailabilityCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<ShowAvailabilityDto>> Handle(CreateShowAvailabilityCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<ShowAvailability>(request);
  //              
  //              // await _unitOfWork.ShowAvailabilitys.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<ShowAvailabilityDto>(entity);
  //              // return BaseResponse<ShowAvailabilityDto>.Ok(dto, "ShowAvailability created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<ShowAvailabilityDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

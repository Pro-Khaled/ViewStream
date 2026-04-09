using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Device.CreateDevice
{
  //  public class CreateDeviceCommandHandler : IRequestHandler<CreateDeviceCommand, BaseResponse<DeviceDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateDeviceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<DeviceDto>> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Device>(request);
  //              
  //              // await _unitOfWork.Devices.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<DeviceDto>(entity);
  //              // return BaseResponse<DeviceDto>.Ok(dto, "Device created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<DeviceDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}

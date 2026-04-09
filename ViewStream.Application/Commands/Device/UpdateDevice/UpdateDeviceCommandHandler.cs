using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Device.UpdateDevice
{
//    public class UpdateDeviceCommandHandler : IRequestHandler<UpdateDeviceCommand, BaseResponse<DeviceDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateDeviceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<DeviceDto>> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Devices.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<DeviceDto>.Fail("Device not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Devices.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<DeviceDto>(entity);
//                // return BaseResponse<DeviceDto>.Ok(dto, "Device updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<DeviceDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}

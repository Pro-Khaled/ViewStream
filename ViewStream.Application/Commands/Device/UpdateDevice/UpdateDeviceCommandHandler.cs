using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Device.UpdateDevice
{
    public class UpdateDeviceCommandHandler : IRequestHandler<UpdateDeviceCommand, DeviceDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateDeviceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DeviceDto?> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
        {
            var device = await _unitOfWork.Devices.GetByIdAsync<long>(request.Id, cancellationToken);
            if (device == null || device.UserId != request.UserId) return null;

            _mapper.Map(request.Dto, device);
            _unitOfWork.Devices.Update(device);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<DeviceDto>(device);
        }
    }
}

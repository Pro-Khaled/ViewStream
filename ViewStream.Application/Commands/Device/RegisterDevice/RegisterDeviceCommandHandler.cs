using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Device.CreateDevice
{
    using Device = ViewStream.Domain.Entities.Device;
    public class RegisterDeviceCommandHandler : IRequestHandler<RegisterDeviceCommand, DeviceDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RegisterDeviceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DeviceDto> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
        {
            var device = _mapper.Map<Device>(request.Dto);
            device.UserId = request.UserId;
            device.LastActive = DateTime.UtcNow;
            device.IsTrusted = false;

            await _unitOfWork.Devices.AddAsync(device, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<DeviceDto>(device);
        }
    }
}

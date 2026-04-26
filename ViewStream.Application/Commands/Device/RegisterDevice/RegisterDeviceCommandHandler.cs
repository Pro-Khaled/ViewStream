using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Device.CreateDevice
{
    using Device = ViewStream.Domain.Entities.Device;
    public class RegisterDeviceCommandHandler : IRequestHandler<RegisterDeviceCommand, DeviceDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RegisterDeviceCommandHandler> _logger;

        public RegisterDeviceCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RegisterDeviceCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<DeviceDto> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering device for UserId: {UserId}, DeviceId: {DeviceId}",
                request.UserId, request.Dto.DeviceId);

            var device = _mapper.Map<Device>(request.Dto);
            device.UserId = request.UserId;
            device.LastActive = DateTime.UtcNow;
            device.IsTrusted = false;

            await _unitOfWork.Devices.AddAsync(device, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Device, object>(
                tableName: "Devices",
                recordId: device.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Device registered with Id: {DeviceId}", device.Id);
            return _mapper.Map<DeviceDto>(device);
        }
    }
}

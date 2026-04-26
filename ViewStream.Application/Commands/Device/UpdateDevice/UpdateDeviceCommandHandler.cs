using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Device.UpdateDevice
{
    using Device = ViewStream.Domain.Entities.Device;
    public class UpdateDeviceCommandHandler : IRequestHandler<UpdateDeviceCommand, DeviceDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateDeviceCommandHandler> _logger;

        public UpdateDeviceCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateDeviceCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<DeviceDto?> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating device with Id: {DeviceId} for UserId: {UserId}",
                request.Id, request.UserId);

            var device = await _unitOfWork.Devices.GetByIdAsync<long>(request.Id, cancellationToken);
            if (device == null || device.UserId != request.UserId)
            {
                _logger.LogWarning("Device not found or access denied. Id: {DeviceId}, UserId: {UserId}",
                    request.Id, request.UserId);
                return null;
            }

            var oldValues = _mapper.Map<DeviceDto>(device);
            _mapper.Map(request.Dto, device);
            _unitOfWork.Devices.Update(device);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Device, object>(
                tableName: "Devices",
                recordId: device.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Device updated with Id: {DeviceId}", device.Id);
            return _mapper.Map<DeviceDto>(device);
        }
    }
}

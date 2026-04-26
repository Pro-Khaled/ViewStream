using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Device.DeleteDevice
{
    using Device = ViewStream.Domain.Entities.Device;
    public class DeleteDeviceCommandHandler : IRequestHandler<DeleteDeviceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteDeviceCommandHandler> _logger;

        public DeleteDeviceCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteDeviceCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting device with Id: {DeviceId} for UserId: {UserId}",
                request.Id, request.UserId);

            var device = await _unitOfWork.Devices.GetByIdAsync<long>(request.Id, cancellationToken);
            if (device == null || device.UserId != request.UserId)
            {
                _logger.LogWarning("Device not found or access denied. Id: {DeviceId}, UserId: {UserId}",
                    request.Id, request.UserId);
                return false;
            }

            var oldValues = _mapper.Map<DeviceDto>(device);
            _unitOfWork.Devices.Delete(device);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Device, object>(
                tableName: "Devices",
                recordId: request.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Device deleted with Id: {DeviceId}", request.Id);
            return true;
        }
    }
}

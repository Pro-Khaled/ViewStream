using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Device.DeleteDeviceAdmin
{
    using Device = ViewStream.Domain.Entities.Device;
    public class DeleteDeviceAdminCommandHandler : IRequestHandler<DeleteDeviceAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteDeviceAdminCommandHandler> _logger;

        public DeleteDeviceAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteDeviceAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteDeviceAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin deleting device Id: {DeviceId} by AdminUserId: {AdminUserId}", request.Id, request.AdminUserId);

            var device = await _unitOfWork.Devices.GetByIdAsync<long>(request.Id, cancellationToken);
            if (device == null)
            {
                _logger.LogWarning("Device not found. Id: {DeviceId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<DeviceDto>(device);
            _unitOfWork.Devices.Delete(device);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Device, object>(
                tableName: "Devices",
                recordId: request.Id,
                action: "DELETE_BY_ADMIN",
                oldValues: oldValues,
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Device hard-deleted by admin. Id: {DeviceId}", request.Id);
            return true;
        }
    }
}

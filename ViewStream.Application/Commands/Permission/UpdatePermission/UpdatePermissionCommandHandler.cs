using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Permission.UpdatePermission
{
    using Permission = ViewStream.Domain.Entities.Permission;
    public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, PermissionDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdatePermissionCommandHandler> _logger;

        public UpdatePermissionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdatePermissionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<PermissionDto?> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating permission Id: {PermissionId}", request.Id);

            var permission = await _unitOfWork.Permissions.GetByIdAsync<int>(request.Id, cancellationToken);
            if (permission == null)
            {
                _logger.LogWarning("Permission not found. Id: {PermissionId}", request.Id);
                return null;
            }

            var oldValues = _mapper.Map<PermissionDto>(permission);
            _mapper.Map(request.Dto, permission);
            _unitOfWork.Permissions.Update(permission);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Permission, object>(
                tableName: "Permissions",
                recordId: permission.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Permission updated. Id: {PermissionId}", permission.Id);
            return _mapper.Map<PermissionDto>(permission);
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Permission.DeletePermission
{
    using Permission = ViewStream.Domain.Entities.Permission;
    public class DeletePermissionCommandHandler : IRequestHandler<DeletePermissionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeletePermissionCommandHandler> _logger;

        public DeletePermissionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeletePermissionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting permission Id: {PermissionId}", request.Id);

            var permission = await _unitOfWork.Permissions.GetByIdAsync<int>(request.Id, cancellationToken);
            if (permission == null)
            {
                _logger.LogWarning("Permission not found. Id: {PermissionId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<PermissionDto>(permission);
            _unitOfWork.Permissions.Delete(permission);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Permission, object>(
                tableName: "Permissions",
                recordId: request.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Permission deleted. Id: {PermissionId}", request.Id);
            return true;
        }
    }
}

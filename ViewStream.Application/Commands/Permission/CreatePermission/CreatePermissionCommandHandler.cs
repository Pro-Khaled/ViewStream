using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Permission.CreatePermission
{
    using Permission = ViewStream.Domain.Entities.Permission;
    public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, PermissionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreatePermissionCommandHandler> _logger;

        public CreatePermissionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreatePermissionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<PermissionDto> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating permission: {PermissionName}", request.Dto.Name);

            var permission = _mapper.Map<Permission>(request.Dto);
            permission.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Permissions.AddAsync(permission, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Permission, object>(
                tableName: "Permissions",
                recordId: permission.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Permission created with Id: {PermissionId}", permission.Id);
            return _mapper.Map<PermissionDto>(permission);
        }
    }
}

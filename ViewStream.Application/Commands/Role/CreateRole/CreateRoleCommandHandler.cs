using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Role.CreateRole
{
    using Role = ViewStream.Domain.Entities.Role;
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateRoleCommandHandler> _logger;

        public CreateRoleCommandHandler(
            RoleManager<Role> roleManager,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateRoleCommandHandler> logger)
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating role: {RoleName}", request.Dto.Name);

            var role = new Role
            {
                Name = request.Dto.Name,
                Description = request.Dto.Description,
                IsSystem = false,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Assign permissions via many-to-many navigation property
            if (request.Dto.PermissionIds != null && request.Dto.PermissionIds.Any())
            {
                var permissions = await _unitOfWork.Permissions.FindAsync(
                    p => request.Dto.PermissionIds.Contains(p.Id),
                    asNoTracking: false,
                    cancellationToken: cancellationToken);

                foreach (var perm in permissions)
                    role.Permissions.Add(perm);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            _auditContext.SetAudit<Role, object>(
                tableName: "Roles",
                recordId: role.Id,
                action: "INSERT",
                oldValues: null,
                newValues: new { role.Name, role.Description },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Role created with Id: {RoleId}", role.Id);
            return _mapper.Map<RoleDto>(role);
        }
    }
}

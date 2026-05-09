using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;


namespace ViewStream.Application.Commands.Role.UpdateRole
{
    using Role = ViewStream.Domain.Entities.Role;
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto?>
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;

        public UpdateRoleCommandHandler(
            RoleManager<Role> roleManager,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateRoleCommandHandler> logger)
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<RoleDto?> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating role Id: {RoleId}", request.Id);

            // Load role with permissions eager-loaded so we can replace the collection
            var roles = await _unitOfWork.Roles.FindAsync(
                r => r.Id == request.Id,
                include: q => q.Include(r => r.Permissions),
                asNoTracking: false,
                cancellationToken: cancellationToken);

            var role = roles.FirstOrDefault();
            if (role == null || role.IsSystem)
            {
                _logger.LogWarning("Role not found or is a system role. Id: {RoleId}", request.Id);
                return null;
            }

            var oldValues = _mapper.Map<RoleDto>(role);
            role.Description = request.Dto.Description;
            role.UpdatedAt = DateTime.UtcNow;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Reassign permissions — replace entire collection
            if (request.Dto.PermissionIds != null)
            {
                role.Permissions.Clear();

                if (request.Dto.PermissionIds.Any())
                {
                    var permissions = await _unitOfWork.Permissions.FindAsync(
                        p => request.Dto.PermissionIds.Contains(p.Id),
                        asNoTracking: false,
                        cancellationToken: cancellationToken);

                    foreach (var perm in permissions)
                        role.Permissions.Add(perm);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            _auditContext.SetAudit<Role, object>(
                tableName: "Roles",
                recordId: role.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: new { role.Description },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Role updated. Id: {RoleId}", role.Id);
            return _mapper.Map<RoleDto>(role);
        }
    }
}

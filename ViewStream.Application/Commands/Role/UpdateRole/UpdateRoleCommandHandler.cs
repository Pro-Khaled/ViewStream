using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;


namespace ViewStream.Application.Commands.Role.UpdateRole
{
    using Role = ViewStream.Domain.Entities.Role;
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto?>
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;

        public UpdateRoleCommandHandler(
            RoleManager<Role> roleManager,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateRoleCommandHandler> logger)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<RoleDto?> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating role Id: {RoleId}", request.Id);

            var role = await _roleManager.FindByIdAsync(request.Id.ToString());
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

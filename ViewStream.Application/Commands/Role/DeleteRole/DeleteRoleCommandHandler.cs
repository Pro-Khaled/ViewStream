using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;


namespace ViewStream.Application.Commands.Role.DeleteRole
{
    using Role = ViewStream.Domain.Entities.Role;
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteRoleCommandHandler> _logger;

        public DeleteRoleCommandHandler(
            RoleManager<Role> roleManager,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteRoleCommandHandler> logger)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting role Id: {RoleId}", request.Id);

            var role = await _roleManager.FindByIdAsync(request.Id.ToString());
            if (role == null || role.IsSystem)
            {
                _logger.LogWarning("Role not found or is a system role. Id: {RoleId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<RoleDto>(role);
            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                return false;

            _auditContext.SetAudit<Role, object>(
                tableName: "Roles",
                recordId: request.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Role deleted. Id: {RoleId}", request.Id);
            return true;
        }
    }
}

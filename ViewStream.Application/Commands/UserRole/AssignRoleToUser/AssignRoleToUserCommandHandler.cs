using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Commands.UserRole.AssignRoleToUser
{
    using Role = ViewStream.Domain.Entities.Role;
    using User = ViewStream.Domain.Entities.User;
    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, UserRoleDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<AssignRoleToUserCommandHandler> _logger;

        public AssignRoleToUserCommandHandler(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<AssignRoleToUserCommandHandler> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<UserRoleDto> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Assigning RoleId: {RoleId} to UserId: {UserId} by AdminUserId: {AdminUserId}",
                request.Dto.RoleId, request.UserId, request.ActorUserId);

            var user = await _userManager.FindByIdAsync(request.UserId.ToString())
                ?? throw new InvalidOperationException("User not found.");
            var role = await _roleManager.FindByIdAsync(request.Dto.RoleId.ToString())
                ?? throw new InvalidOperationException("Role not found.");

            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors));

            _auditContext.SetAudit<object, object>(
                tableName: "UserRoles",
                recordId: user.Id.GetHashCode() ^ role.Id,
                action: "INSERT",
                oldValues: null,
                newValues: new { UserId = user.Id, RoleId = role.Id, RoleName = role.Name },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Role {RoleName} assigned to User {UserId}", role.Name, user.Id);
            return new UserRoleDto { UserId = user.Id, UserEmail = user.Email, RoleId = role.Id, RoleName = role.Name };
        }
    }
}

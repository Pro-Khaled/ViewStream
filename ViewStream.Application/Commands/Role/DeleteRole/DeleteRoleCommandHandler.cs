using MediatR;
using Microsoft.AspNetCore.Identity;


namespace ViewStream.Application.Commands.Role.DeleteRole
{
    using Role = ViewStream.Domain.Entities.Role;
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly RoleManager<Role> _roleManager;
        public DeleteRoleCommandHandler(RoleManager<Role> roleManager) => _roleManager = roleManager;
        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id.ToString());
            if (role == null || role.IsSystem) return false;
            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }
    }
}

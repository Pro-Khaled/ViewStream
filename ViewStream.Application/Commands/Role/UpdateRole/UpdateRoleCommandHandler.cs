using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ViewStream.Application.DTOs;


namespace ViewStream.Application.Commands.Role.UpdateRole
{
    using Role = ViewStream.Domain.Entities.Role;
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto?>
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public UpdateRoleCommandHandler(RoleManager<Role> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<RoleDto?> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id.ToString());
            if (role == null || role.IsSystem) return null;
            role.Description = request.Dto.Description;
            role.UpdatedAt = DateTime.UtcNow;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded ? _mapper.Map<RoleDto>(role) : null;
        }
    }
}

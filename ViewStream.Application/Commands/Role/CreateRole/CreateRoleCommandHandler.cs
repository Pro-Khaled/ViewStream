using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Role.CreateRole
{
    using Role = ViewStream.Domain.Entities.Role;
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public CreateRoleCommandHandler(RoleManager<Role> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new Role { Name = request.Dto.Name, Description = request.Dto.Description, IsSystem = false, CreatedAt = DateTime.UtcNow };
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join(", ", result.Errors));

            if (request.Dto.PermissionIds.Any())
            {
                var permissions = await _roleManager.Roles.Where(r => r.Id == role.Id).SelectMany(r => r.Permissions).ToListAsync(cancellationToken);
                // Requires explicit Permission repository or RoleManager integration – simplified for brevity
            }

            return _mapper.Map<RoleDto>(role);
        }
    }
}

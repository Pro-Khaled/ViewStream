using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserRole.AssignRoleToUser
{
    using User = ViewStream.Domain.Entities.User;
    using Role = ViewStream.Domain.Entities.Role;
    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, UserRoleDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        public AssignRoleToUserCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper)
        { _userManager = userManager; _roleManager = roleManager; _mapper = mapper; }

        public async Task<UserRoleDto> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new InvalidOperationException("User not found.");
            var role = await _roleManager.FindByIdAsync(request.Dto.RoleId.ToString()) ?? throw new InvalidOperationException("Role not found.");
            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join(", ", result.Errors));
            return new UserRoleDto { UserId = user.Id, UserEmail = user.Email, RoleId = role.Id, RoleName = role.Name };
        }
    }
}

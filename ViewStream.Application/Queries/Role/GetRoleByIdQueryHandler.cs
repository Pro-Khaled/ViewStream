using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;


namespace ViewStream.Application.Queries.Role
{
    using Role = ViewStream.Domain.Entities.Role;
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto?>
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        public GetRoleByIdQueryHandler(RoleManager<Role> roleManager, IMapper mapper) { _roleManager = roleManager; _mapper = mapper; }
        public async Task<RoleDto?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            return role == null ? null : _mapper.Map<RoleDto>(role);
        }
    }
}

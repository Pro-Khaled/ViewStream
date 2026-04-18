using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;


namespace ViewStream.Application.Queries.Role
{
    using Role = ViewStream.Domain.Entities.Role;
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleListItemDto>>
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        public GetAllRolesQueryHandler(RoleManager<Role> roleManager, IMapper mapper) { _roleManager = roleManager; _mapper = mapper; }
        public async Task<List<RoleListItemDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.OrderBy(r => r.Name).AsNoTracking().ToListAsync(cancellationToken);
            return _mapper.Map<List<RoleListItemDto>>(roles);
        }
    }
}

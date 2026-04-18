using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Permission
{
    public class GetPermissionsByGroupQueryHandler : IRequestHandler<GetPermissionsByGroupQuery, Dictionary<string, List<PermissionListItemDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPermissionsByGroupQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Dictionary<string, List<PermissionListItemDto>>> Handle(GetPermissionsByGroupQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _unitOfWork.Permissions.GetQueryable()
                .OrderBy(p => p.GroupName).ThenBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return permissions
                .GroupBy(p => p.GroupName ?? "Other")
                .ToDictionary(g => g.Key, g => _mapper.Map<List<PermissionListItemDto>>(g.ToList()));
        }
    }
}

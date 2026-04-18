using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Permission
{
    public record GetPermissionsByGroupQuery : IRequest<Dictionary<string, List<PermissionListItemDto>>>;

}

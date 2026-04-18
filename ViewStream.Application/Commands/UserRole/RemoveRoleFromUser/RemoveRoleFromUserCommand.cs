using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.UserRole.RemoveRoleFromUser
{
    public record RemoveRoleFromUserCommand(long UserId, long RoleId) : IRequest<bool>;

}

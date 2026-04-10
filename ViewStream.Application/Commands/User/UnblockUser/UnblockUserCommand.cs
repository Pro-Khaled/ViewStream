using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.User.UnblockUser
{
    // Admin: Unblock user
    public record UnblockUserCommand(long UserId, long AdminUserId) : IRequest<bool>;
}

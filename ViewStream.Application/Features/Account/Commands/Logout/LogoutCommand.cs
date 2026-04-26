using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Features.Account.Commands.Logout
{
    public record LogoutCommand(string RefreshToken, long UserId) : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}

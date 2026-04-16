using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.LoginSession.RevokeAllUserSessions
{
    public record RevokeAllUserSessionsCommand(long UserId) : IRequest<bool>;

}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.LoginSession.RevokeLoginSession
{
    public record RevokeLoginSessionCommand(long Id, long UserId) : IRequest<bool>;

}

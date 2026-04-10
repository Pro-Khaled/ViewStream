using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Features.Account.Commands.Logout
{
    public record LogoutCommand(string RefreshToken) : IRequest<bool>;

}

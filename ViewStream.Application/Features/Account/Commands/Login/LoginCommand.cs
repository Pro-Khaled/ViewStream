using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs.Account;

namespace ViewStream.Application.Features.Account.Commands.Login
{
    public record LoginCommand(LoginDto Dto) : IRequest<AuthResponseDto>;
}

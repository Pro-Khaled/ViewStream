using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.Account.ConfirmEmail
{
    public record ConfirmEmailCommand(long UserId, string Token) : IRequest<ConfirmEmailResult>;

    public record ConfirmEmailResult(bool Succeeded, string? ErrorMessage = null);
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.SharedList.GenerateShareCode
{
    public record GenerateShareCodeCommand(long Id, long OwnerProfileId) : IRequest<string?>;

}

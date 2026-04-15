using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.PersonAward.RemovePersonAward
{
    public record RemovePersonAwardCommand(long PersonId, int AwardId) : IRequest<bool>;

}

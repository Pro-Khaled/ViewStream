using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.ShowAward.RemoveShowAward
{
    public record RemoveShowAwardCommand(long ShowId, int AwardId) : IRequest<bool>;

}

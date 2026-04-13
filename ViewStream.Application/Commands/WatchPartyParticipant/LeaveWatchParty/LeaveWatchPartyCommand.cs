using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.WatchPartyParticipant.LeaveWatchParty
{
    public record LeaveWatchPartyCommand(long PartyId, long ProfileId) : IRequest<bool>;

}

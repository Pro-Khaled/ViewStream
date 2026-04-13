using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.WatchPartyParticipant.JoinWatchParty
{
    public record JoinWatchPartyCommand(long PartyId, long ProfileId) : IRequest<WatchPartyParticipantDto>;

}

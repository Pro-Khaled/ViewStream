using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.WatchParty.UpdateWatchParty
{
    public record UpdateWatchPartyCommand(long Id, long ProfileId, UpdateWatchPartyDto Dto) : IRequest<WatchPartyDto?>;

}

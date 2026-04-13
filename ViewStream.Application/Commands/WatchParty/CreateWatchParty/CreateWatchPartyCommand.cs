using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.WatchParty.CreateWatchParty
{
    public record CreateWatchPartyCommand(long HostProfileId, CreateWatchPartyDto Dto) : IRequest<WatchPartyDto>;

}

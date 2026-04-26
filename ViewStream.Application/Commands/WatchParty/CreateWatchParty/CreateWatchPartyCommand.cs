using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.WatchParty.CreateWatchParty
{
    public record CreateWatchPartyCommand(long HostProfileId, CreateWatchPartyDto Dto, long ActorUserId)
        : IRequest<WatchPartyDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

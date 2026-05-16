using MediatR;

namespace ViewStream.Application.Commands.WatchParty.ForceCloseWatchParty
{
    public record ForceCloseWatchPartyCommand(long Id, long AdminUserId) : IRequest<bool>;
}

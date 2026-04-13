using MediatR;

namespace ViewStream.Application.Commands.WatchParty.DeleteWatchParty
{
    public record EndWatchPartyCommand(long Id, long ProfileId) : IRequest<bool>;

}

using MediatR;


namespace ViewStream.Application.Commands.Season.RestoreSeason
{
    public record RestoreSeasonCommand(long Id, long RestoredByUserId) : IRequest<bool>;

}

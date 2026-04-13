using MediatR;


namespace ViewStream.Application.Commands.Season.DeleteSeason
{
    public record DeleteSeasonCommand(long Id, long DeletedByUserId) : IRequest<bool>;

}

using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Season.UpdateSeason
{
    public record UpdateSeasonCommand(long Id, UpdateSeasonDto Dto, long UpdatedByUserId) : IRequest<bool>;
}

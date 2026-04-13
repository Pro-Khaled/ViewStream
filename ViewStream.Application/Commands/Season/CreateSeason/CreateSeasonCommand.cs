using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Season.CreateSeason
{
    public record CreateSeasonCommand(CreateSeasonDto Dto, long CreatedByUserId) : IRequest<long>;

}

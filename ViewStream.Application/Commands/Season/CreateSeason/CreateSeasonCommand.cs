using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Season.CreateSeason
{
    public record CreateSeasonCommand(CreateSeasonDto Dto, long ActorUserId)
        : IRequest<long>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

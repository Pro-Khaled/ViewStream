using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Award.UpdateAward
{
    public record UpdateAwardCommand(int Id, UpdateAwardDto Dto, long UpdatedByUserId)
        : IRequest<AwardDto?>, IHasUserId
    {
        public long? UserId => UpdatedByUserId;
    }
}

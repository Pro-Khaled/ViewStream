using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Award.CreateAward
{
    public record CreateAwardCommand(CreateAwardDto Dto, long CreatedByUserId)
        : IRequest<AwardDto>, IHasUserId
    {
        public long? UserId => CreatedByUserId;
    }
}

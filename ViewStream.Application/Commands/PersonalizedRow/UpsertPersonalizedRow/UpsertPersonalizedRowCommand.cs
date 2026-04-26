using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PersonalizedRow.UpsertPersonalizedRow
{
    public record UpsertPersonalizedRowCommand(long ProfileId, CreateUpdatePersonalizedRowDto Dto, long ActorUserId)
        : IRequest<PersonalizedRowDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

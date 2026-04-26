using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PersonAward.AddPersonAward
{
    public record AddPersonAwardCommand(long PersonId, CreatePersonAwardDto Dto, long ActorUserId)
        : IRequest<PersonAwardDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Person.CreatePerson
{
    public record CreatePersonCommand(CreatePersonDto Dto, long ActorUserId)
        : IRequest<PersonDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Person.UpdatePerson
{
    public record UpdatePersonCommand(long Id, UpdatePersonDto Dto, long ActorUserId)
        : IRequest<PersonDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

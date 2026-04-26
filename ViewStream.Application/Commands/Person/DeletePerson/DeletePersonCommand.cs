using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Person.DeletePerson
{
    public record DeletePersonCommand(long Id, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

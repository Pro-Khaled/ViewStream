using MediatR;

namespace ViewStream.Application.Commands.Person.DeletePerson
{
    public record DeletePersonCommand(long Id) : IRequest<bool>; // Hard delete

}

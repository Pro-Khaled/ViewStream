using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Person.CreatePerson
{
    public record CreatePersonCommand(CreatePersonDto Dto) : IRequest<PersonDto>;

}

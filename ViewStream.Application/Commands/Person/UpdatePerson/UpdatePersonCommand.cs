using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Person.UpdatePerson
{
    public record UpdatePersonCommand(long Id, UpdatePersonDto Dto) : IRequest<PersonDto?>;

}

using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Person
{
    public record GetPersonByIdQuery(long Id) : IRequest<PersonDto?>;

}

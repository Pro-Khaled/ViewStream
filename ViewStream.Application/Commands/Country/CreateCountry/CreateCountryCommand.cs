using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Country.CreateCountry
{
    public record CreateCountryCommand(CreateCountryDto Dto) : IRequest<string>;

}

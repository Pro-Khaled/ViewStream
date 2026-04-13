using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Country.UpdateCountry
{
    public record UpdateCountryCommand(string Code, UpdateCountryDto Dto) : IRequest<bool>;

}

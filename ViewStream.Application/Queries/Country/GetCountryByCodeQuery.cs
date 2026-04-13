using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Country
{
    public record GetCountryByCodeQuery(string Code) : IRequest<CountryDto?>;

}

using MediatR;

namespace ViewStream.Application.Commands.Country.DeleteCountry
{
    public record DeleteCountryCommand(string Code) : IRequest<bool>; // Hard delete (cascades to ShowAvailability)

}

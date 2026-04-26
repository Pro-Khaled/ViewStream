using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Country.CreateCountry
{
    public record CreateCountryCommand(CreateCountryDto Dto, long UserId)
        : IRequest<string>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}

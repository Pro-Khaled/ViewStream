using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Country.UpdateCountry
{
    public record UpdateCountryCommand(string Code, UpdateCountryDto Dto, long UserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}

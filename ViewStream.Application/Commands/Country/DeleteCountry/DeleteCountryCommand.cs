using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Country.DeleteCountry
{
    public record DeleteCountryCommand(string Code, long UserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}

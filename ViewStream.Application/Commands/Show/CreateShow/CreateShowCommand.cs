using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Show.CreateShow
{
    public record CreateShowCommand(CreateShowDto Dto, long CreatedByUserId)
        : IRequest<long>, IHasUserId
    {
        long? IHasUserId.UserId => CreatedByUserId;
    }
}

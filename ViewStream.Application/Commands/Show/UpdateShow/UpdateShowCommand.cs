using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Show.UpdateShow
{
    public record UpdateShowCommand(long Id, UpdateShowDto Dto, long UpdatedByUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => UpdatedByUserId;
    }
}

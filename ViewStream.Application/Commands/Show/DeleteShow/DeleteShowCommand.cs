using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Show.DeleteShow
{
    public record DeleteShowCommand(long Id, long DeletedByUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => DeletedByUserId;
    }
}

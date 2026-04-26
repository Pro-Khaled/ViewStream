using MediatR;
using ViewStream.Application.Behaviors;


namespace ViewStream.Application.Commands.Show.RestoreShow
{
    public record RestoreShowCommand(long Id, long RestoredByUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => RestoredByUserId;
    }
}

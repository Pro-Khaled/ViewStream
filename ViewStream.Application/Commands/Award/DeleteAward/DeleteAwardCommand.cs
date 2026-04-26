using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Award.DeleteAward
{
    public record DeleteAwardCommand(int Id, long DeletedByUserId)
        : IRequest<bool>, IHasUserId
    {
        public long? UserId => DeletedByUserId;
    }
}

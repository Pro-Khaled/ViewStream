using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.PersonalizedRow.DeletePersonalizedRow
{
    public record DeletePersonalizedRowCommand(long ProfileId, string RowName, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

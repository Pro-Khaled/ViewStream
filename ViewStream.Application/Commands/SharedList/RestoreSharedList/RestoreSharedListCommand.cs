using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.SharedList.RestoreSharedList
{
    using SharedList = ViewStream.Domain.Entities.SharedList;

    /// <summary>
    /// Admin command to restore a soft-deleted shared list.
    /// </summary>
    public record RestoreSharedListCommand(long Id, long RestoredByUserId)
        : IRequest<bool>, IHasUserId
    {
        /// <inheritdoc />
        long? IHasUserId.UserId => RestoredByUserId;
    }
}

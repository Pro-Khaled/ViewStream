using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.User.RestoreUser
{
    /// <summary>
    /// Admin command to restore a soft-deleted user account.
    /// </summary>
    public record RestoreUserCommand(long Id, long RestoredByUserId)
        : IRequest<bool>, IHasUserId
    {
        /// <inheritdoc />
        long? IHasUserId.UserId => RestoredByUserId;
    }
}

using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.SharedList.DeleteSharedListAdmin
{
    public record DeleteSharedListAdminCommand(long Id, long AdminUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => AdminUserId;
    }
}

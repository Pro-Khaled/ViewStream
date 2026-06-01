using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Device.DeleteDeviceAdmin
{
    public record DeleteDeviceAdminCommand(long Id, long AdminUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => AdminUserId;
    }
}

using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Device.DeleteDevice
{
    public record DeleteDeviceCommand(long Id, long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

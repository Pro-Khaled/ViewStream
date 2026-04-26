using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Device.UpdateDevice
{
    public record UpdateDeviceCommand(long Id, long UserId, UpdateDeviceDto Dto, long ActorUserId)
        : IRequest<DeviceDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

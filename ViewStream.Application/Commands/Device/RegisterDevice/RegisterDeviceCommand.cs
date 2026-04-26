using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Device.CreateDevice
{
    public record RegisterDeviceCommand(long UserId, CreateDeviceDto Dto, long ActorUserId)
        : IRequest<DeviceDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

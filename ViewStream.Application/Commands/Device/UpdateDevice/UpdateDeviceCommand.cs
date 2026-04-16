using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Device.UpdateDevice
{
    public record UpdateDeviceCommand(long Id, long UserId, UpdateDeviceDto Dto) : IRequest<DeviceDto?>;

}

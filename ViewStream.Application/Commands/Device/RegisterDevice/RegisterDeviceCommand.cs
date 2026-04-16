using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Device.CreateDevice
{
    public record RegisterDeviceCommand(long UserId, CreateDeviceDto Dto) : IRequest<DeviceDto>;

}

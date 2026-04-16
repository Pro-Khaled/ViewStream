using MediatR;

namespace ViewStream.Application.Commands.Device.DeleteDevice
{
    public record DeleteDeviceCommand(long Id, long UserId) : IRequest<bool>;

}

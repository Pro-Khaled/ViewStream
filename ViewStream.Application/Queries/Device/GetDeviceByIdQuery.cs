using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Device
{
    public record GetDeviceByIdQuery(long Id, long UserId) : IRequest<DeviceDto?>;

}

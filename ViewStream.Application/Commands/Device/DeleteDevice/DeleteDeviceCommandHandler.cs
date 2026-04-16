using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Device.DeleteDevice
{
    public class DeleteDeviceCommandHandler : IRequestHandler<DeleteDeviceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDeviceCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
        {
            var device = await _unitOfWork.Devices.GetByIdAsync<long>(request.Id, cancellationToken);
            if (device == null || device.UserId != request.UserId) return false;

            _unitOfWork.Devices.Delete(device);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

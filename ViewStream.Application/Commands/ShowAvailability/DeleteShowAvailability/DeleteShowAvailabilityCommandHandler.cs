using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAvailability.DeleteShowAvailability
{
    public class DeleteShowAvailabilityCommandHandler : IRequestHandler<DeleteShowAvailabilityCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteShowAvailabilityCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteShowAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var availability = await _unitOfWork.ShowAvailabilities.FindAsync(
                predicate: sa => sa.ShowId == request.ShowId && sa.CountryCode == request.CountryCode,
                cancellationToken: cancellationToken);

            var entity = availability.FirstOrDefault();
            if (entity == null) return false;

            _unitOfWork.ShowAvailabilities.Delete(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

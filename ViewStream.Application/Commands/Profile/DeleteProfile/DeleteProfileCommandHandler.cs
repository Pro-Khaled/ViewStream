using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Profile.DeleteProfile
{
    public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProfileCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _unitOfWork.Profiles.GetByIdAsync<long>(request.Id, cancellationToken);
            if (profile == null || profile.UserId != request.UserId || profile.IsDeleted == true)
                return false;

            profile.IsDeleted = true;
            profile.DeletedAt = DateTime.UtcNow;
            profile.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

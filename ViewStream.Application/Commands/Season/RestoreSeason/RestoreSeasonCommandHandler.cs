using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Season.RestoreSeason
{
    public class RestoreSeasonCommandHandler : IRequestHandler<RestoreSeasonCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RestoreSeasonCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RestoreSeasonCommand request, CancellationToken cancellationToken)
        {
            var season = await _unitOfWork.Seasons.GetByIdAsync<long>(request.Id, cancellationToken);
            if (season == null || season.IsDeleted != true) return false;

            season.IsDeleted = false;
            season.DeletedAt = null;
            season.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

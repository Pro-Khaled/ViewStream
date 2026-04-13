using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Season.DeleteSeason
{
    public class DeleteSeasonCommandHandler : IRequestHandler<DeleteSeasonCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSeasonCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSeasonCommand request, CancellationToken cancellationToken)
        {
            var season = await _unitOfWork.Seasons.GetByIdAsync<long>(request.Id, cancellationToken);
            if (season == null || season.IsDeleted == true) return false;

            season.IsDeleted = true;
            season.DeletedAt = DateTime.UtcNow;
            season.UpdatedAt = DateTime.UtcNow;

            var episodes = await _unitOfWork.Episodes.FindAsync(e => e.SeasonId == request.Id, cancellationToken: cancellationToken);
            foreach (var episode in episodes)
            {
                episode.IsDeleted = true;
                episode.DeletedAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

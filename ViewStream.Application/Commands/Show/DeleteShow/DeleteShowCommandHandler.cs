using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.DeleteShow
{
    public class DeleteShowCommandHandler : IRequestHandler<DeleteShowCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteShowCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteShowCommand request, CancellationToken cancellationToken)
        {
            var show = await _unitOfWork.Shows.GetByIdAsync<long>(request.Id, cancellationToken);
            if (show == null || show.IsDeleted == true) return false;

            show.IsDeleted = true;
            show.DeletedAt = DateTime.UtcNow;
            show.UpdatedAt = DateTime.UtcNow;

            // Soft delete seasons and episodes (cascade)
            var seasons = await _unitOfWork.Seasons.FindAsync(s => s.ShowId == request.Id, cancellationToken: cancellationToken);
            foreach (var season in seasons)
            {
                season.IsDeleted = true;
                season.DeletedAt = DateTime.UtcNow;
                var episodes = await _unitOfWork.Episodes.FindAsync(e => e.SeasonId == season.Id, cancellationToken: cancellationToken);
                foreach (var episode in episodes)
                {
                    episode.IsDeleted = true;
                    episode.DeletedAt = DateTime.UtcNow;
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

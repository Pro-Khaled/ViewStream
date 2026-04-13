using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.DeleteEpisode
{
    public class DeleteEpisodeCommandHandler : IRequestHandler<DeleteEpisodeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEpisodeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteEpisodeCommand request, CancellationToken cancellationToken)
        {
            var episode = await _unitOfWork.Episodes.GetByIdAsync<long>(request.Id, cancellationToken);
            if (episode == null || episode.IsDeleted == true)
                return false;

            episode.IsDeleted = true;
            episode.DeletedAt = DateTime.UtcNow;
            episode.UpdatedAt = DateTime.UtcNow;

            var audioTracks = await _unitOfWork.AudioTracks.FindAsync(a => a.EpisodeId == request.Id, cancellationToken: cancellationToken);
            foreach (var audio in audioTracks)
                audio.IsDeleted = true;

            var subtitles = await _unitOfWork.Subtitles.FindAsync(s => s.EpisodeId == request.Id, cancellationToken: cancellationToken);
            foreach (var subtitle in subtitles)
                subtitle.IsDeleted = true;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

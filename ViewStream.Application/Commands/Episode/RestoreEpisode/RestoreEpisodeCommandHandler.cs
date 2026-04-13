using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.RestoreEpisode
{
    public class RestoreEpisodeCommandHandler : IRequestHandler<RestoreEpisodeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RestoreEpisodeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RestoreEpisodeCommand request, CancellationToken cancellationToken)
        {
            var episode = await _unitOfWork.Episodes.GetByIdAsync<long>(request.Id, cancellationToken);
            if (episode == null || episode.IsDeleted != true)
                return false;

            episode.IsDeleted = false;
            episode.DeletedAt = null;
            episode.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

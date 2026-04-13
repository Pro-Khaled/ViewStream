using MediatR;
using Microsoft.AspNetCore.SignalR;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Application.Queries.Episode;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Episode.UploadEpisodeThumbnail
{
    public class UploadEpisodeThumbnailCommandHandler : IRequestHandler<UploadEpisodeThumbnailCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IMediator _mediator;
        private readonly IEpisodeHubClient _hubClient;

        public UploadEpisodeThumbnailCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IMediator mediator,
            IEpisodeHubClient hubClient)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _mediator = mediator;
            _hubClient = hubClient;
        }

        public async Task<string> Handle(UploadEpisodeThumbnailCommand request, CancellationToken cancellationToken)
        {
            // 1. Retrieve the episode entity
            var entity = await _unitOfWork.Episodes.GetByIdAsync<long>(request.EpisodeId, cancellationToken);
            if (entity == null)
                throw new InvalidOperationException("Episode not found.");

            // 2. Delete old thumbnail if exists
            if (!string.IsNullOrEmpty(entity.ThumbnailUrl))
                _fileStorage.DeleteFile(entity.ThumbnailUrl);

            // 3. Save new thumbnail file
            var thumbnailUrl = await _fileStorage.SaveThumbnailAsync(request.ThumbnailFile, request.EpisodeId, cancellationToken);

            // 4. Update entity
            entity.ThumbnailUrl = thumbnailUrl;
            entity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Episodes.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Get updated DTO for SignalR payload
            var updatedEpisodeDto = await _mediator.Send(new GetEpisodeByIdQuery(request.EpisodeId), cancellationToken);

            // 6. Notify all clients in the episode's group
            await _hubClient.SendEpisodeThumbnailUpdatedAsync(updatedEpisodeDto, cancellationToken);

            // Also notify general admin dashboard
            await _hubClient.SendEpisodeThumbnailUploadedAsync(
                request.EpisodeId,
                thumbnailUrl,
                request.UploadedByUserId,
                cancellationToken);

            return thumbnailUrl;
        }
    }
}

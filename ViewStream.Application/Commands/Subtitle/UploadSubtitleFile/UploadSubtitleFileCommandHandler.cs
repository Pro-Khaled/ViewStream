using MediatR;
using Microsoft.AspNetCore.SignalR;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Application.Queries.Subtitle;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.UploadSubtitleFile
{
    public class UploadSubtitleFileCommandHandler : IRequestHandler<UploadSubtitleFileCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IMediator _mediator;
        private readonly IEpisodeHubClient _hubClient;

        public UploadSubtitleFileCommandHandler(
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

        public async Task<string> Handle(UploadSubtitleFileCommand request, CancellationToken cancellationToken)
        {
            var subtitle = await _unitOfWork.Subtitles.GetByIdAsync<long>(request.SubtitleId, cancellationToken);
            if (subtitle == null)
                throw new InvalidOperationException("Subtitle not found.");

            if (!string.IsNullOrEmpty(subtitle.SubtitleUrl))
                _fileStorage.DeleteFile(subtitle.SubtitleUrl);

            var fileUrl = await _fileStorage.SaveSubtitleFileAsync(request.File, request.SubtitleId, cancellationToken);
            subtitle.SubtitleUrl = fileUrl;
            subtitle.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Subtitles.Update(subtitle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify clients (optional)
            var updatedSubtitleDto = await _mediator.Send(new GetSubtitleByIdQuery(request.SubtitleId), cancellationToken);
            await _hubClient.SendSubtitleFileUpdatedAsync(updatedSubtitleDto, cancellationToken);
            return fileUrl;
        }
    }
}

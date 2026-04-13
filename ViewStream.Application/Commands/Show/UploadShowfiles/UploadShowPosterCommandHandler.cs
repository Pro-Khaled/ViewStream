using MediatR;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Application.Queries.Show;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.UploadShowfiles
{
    public class UploadShowPosterCommandHandler : IRequestHandler<UploadShowPosterCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IMediator _mediator;
        private readonly IShowHubClient _hubClient;

        public UploadShowPosterCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IMediator mediator,
            IShowHubClient hubClient)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _mediator = mediator;
            _hubClient = hubClient;
        }

        public async Task<string> Handle(UploadShowPosterCommand request, CancellationToken cancellationToken)
        {
            var show = await _unitOfWork.Shows.GetByIdAsync<long>(request.ShowId, cancellationToken);
            if (show == null) throw new InvalidOperationException("Show not found.");

            if (!string.IsNullOrEmpty(show.PosterUrl))
                _fileStorage.DeleteFile(show.PosterUrl);

            var posterUrl = await _fileStorage.SavePosterAsync(request.PosterFile, request.ShowId, cancellationToken);
            show.PosterUrl = posterUrl;
            show.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Shows.Update(show);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedShowDto = await _mediator.Send(new GetShowByIdQuery(request.ShowId), cancellationToken);
            await _hubClient.SendShowPosterUpdatedAsync(updatedShowDto, cancellationToken);

            return posterUrl;
        }
    }
}

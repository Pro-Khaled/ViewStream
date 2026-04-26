using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Application.Queries.Show;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.UploadShowfiles
{
    using Show = ViewStream.Domain.Entities.Show;
    public class UploadShowPosterCommandHandler : IRequestHandler<UploadShowPosterCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IMediator _mediator;
        private readonly IShowHubClient _hubClient;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UploadShowPosterCommandHandler> _logger;

        public UploadShowPosterCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IMediator mediator,
            IShowHubClient hubClient,
            IAuditContext auditContext,
            ILogger<UploadShowPosterCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _mediator = mediator;
            _hubClient = hubClient;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<string> Handle(UploadShowPosterCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Uploading poster for ShowId: {ShowId}", request.ShowId);

            var show = await _unitOfWork.Shows.GetByIdAsync<long>(request.ShowId, cancellationToken);
            if (show == null) throw new InvalidOperationException("Show not found.");

            var oldUrl = show.PosterUrl;
            if (!string.IsNullOrEmpty(oldUrl))
                _fileStorage.DeleteFile(oldUrl);

            var posterUrl = await _fileStorage.SavePosterAsync(request.PosterFile, request.ShowId, cancellationToken);
            show.PosterUrl = posterUrl;
            show.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Shows.Update(show);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Show, object>(
                tableName: "Shows",
                recordId: show.Id,
                action: "UPDATE",
                oldValues: new { PosterUrl = oldUrl },
                newValues: new { PosterUrl = posterUrl },
                changedByUserId: request.UploadedByUserId
            );

            _logger.LogInformation("Poster uploaded for ShowId: {ShowId}, URL: {Url}", show.Id, posterUrl);

            var updatedShowDto = await _mediator.Send(new GetShowByIdQuery(request.ShowId), cancellationToken);
            await _hubClient.SendShowPosterUpdatedAsync(updatedShowDto, cancellationToken);

            return posterUrl;
        }
    }
}

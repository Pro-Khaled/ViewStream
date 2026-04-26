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
    public class UploadShowTrailerCommandHandler : IRequestHandler<UploadShowTrailerCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IMediator _mediator;
        private readonly IShowHubClient _hubClient;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UploadShowTrailerCommandHandler> _logger;

        public UploadShowTrailerCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IMediator mediator,
            IShowHubClient hubClient,
            IAuditContext auditContext,
            ILogger<UploadShowTrailerCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _mediator = mediator;
            _hubClient = hubClient;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<string> Handle(UploadShowTrailerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Uploading trailer for ShowId: {ShowId}", request.ShowId);

            var show = await _unitOfWork.Shows.GetByIdAsync<long>(request.ShowId, cancellationToken);
            if (show == null)
                throw new InvalidOperationException("Show not found.");

            var oldUrl = show.TrailerUrl;
            if (!string.IsNullOrEmpty(oldUrl))
                _fileStorage.DeleteFile(oldUrl);

            var trailerUrl = await _fileStorage.SaveTrailerAsync(request.TrailerFile, request.ShowId, cancellationToken);
            show.TrailerUrl = trailerUrl;
            show.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Shows.Update(show);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Audit log
            _auditContext.SetAudit<Show, object>(
                tableName: "Shows",
                recordId: show.Id,
                action: "UPDATE",
                oldValues: new { TrailerUrl = oldUrl },
                newValues: new { TrailerUrl = trailerUrl },
                changedByUserId: request.UploadedByUserId
            );

            _logger.LogInformation("Trailer uploaded for ShowId: {ShowId}, URL: {Url}", show.Id, trailerUrl);

            // Notify SignalR clients
            var updatedShowDto = await _mediator.Send(new GetShowByIdQuery(request.ShowId), cancellationToken);
            await _hubClient.SendShowTrailerUpdatedAsync(updatedShowDto, cancellationToken);

            return trailerUrl;
        }
    }
}

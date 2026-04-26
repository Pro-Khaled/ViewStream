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
    public class UploadShowBackdropCommandHandler : IRequestHandler<UploadShowBackdropCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IMediator _mediator;
        private readonly IShowHubClient _hubClient;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UploadShowBackdropCommandHandler> _logger;

        public UploadShowBackdropCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IMediator mediator,
            IShowHubClient hubClient,
            IAuditContext auditContext,
            ILogger<UploadShowBackdropCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _mediator = mediator;
            _hubClient = hubClient;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<string> Handle(UploadShowBackdropCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Uploading backdrop for ShowId: {ShowId}", request.ShowId);

            var show = await _unitOfWork.Shows.GetByIdAsync<long>(request.ShowId, cancellationToken);
            if (show == null)
                throw new InvalidOperationException("Show not found.");

            var oldUrl = show.BackdropUrl;
            if (!string.IsNullOrEmpty(oldUrl))
                _fileStorage.DeleteFile(oldUrl);

            var backdropUrl = await _fileStorage.SaveBackdropAsync(request.BackdropFile, request.ShowId, cancellationToken);
            show.BackdropUrl = backdropUrl;
            show.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Shows.Update(show);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Audit log
            _auditContext.SetAudit<Show, object>(
                tableName: "Shows",
                recordId: show.Id,
                action: "UPDATE",
                oldValues: new { BackdropUrl = oldUrl },
                newValues: new { BackdropUrl = backdropUrl },
                changedByUserId: request.UploadedByUserId
            );

            _logger.LogInformation("Backdrop uploaded for ShowId: {ShowId}, URL: {Url}", show.Id, backdropUrl);

            // Notify SignalR clients
            var updatedShowDto = await _mediator.Send(new GetShowByIdQuery(request.ShowId), cancellationToken);
            await _hubClient.SendShowBackdropUpdatedAsync(updatedShowDto, cancellationToken);

            return backdropUrl;
        }
    }
}

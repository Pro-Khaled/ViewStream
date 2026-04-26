using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.RestoreSubtitle
{
    using Subtitle = ViewStream.Domain.Entities.Subtitle;
    public class RestoreSubtitleCommandHandler : IRequestHandler<RestoreSubtitleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RestoreSubtitleCommandHandler> _logger;

        public RestoreSubtitleCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RestoreSubtitleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RestoreSubtitleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Restoring subtitle Id: {SubtitleId}", request.Id);

            var subtitle = await _unitOfWork.Subtitles.GetByIdAsync<long>(request.Id, cancellationToken);
            if (subtitle == null || subtitle.IsDeleted != true)
            {
                _logger.LogWarning("Subtitle not found or not deleted. Id: {SubtitleId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<SubtitleDto>(subtitle);
            subtitle.IsDeleted = false;
            subtitle.DeletedAt = null;
            subtitle.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Subtitle, object>(
                tableName: "Subtitles",
                recordId: subtitle.Id,
                action: "RESTORE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Subtitle restored. Id: {SubtitleId}", subtitle.Id);
            return true;
        }
    }
}

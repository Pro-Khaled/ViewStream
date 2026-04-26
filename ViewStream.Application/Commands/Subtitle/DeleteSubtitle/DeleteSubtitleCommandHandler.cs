using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.DeleteSubtitle
{
    using Subtitle = ViewStream.Domain.Entities.Subtitle;
    public class DeleteSubtitleCommandHandler : IRequestHandler<DeleteSubtitleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteSubtitleCommandHandler> _logger;

        public DeleteSubtitleCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteSubtitleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteSubtitleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting subtitle Id: {SubtitleId}", request.Id);

            var subtitle = await _unitOfWork.Subtitles.GetByIdAsync<long>(request.Id, cancellationToken);
            if (subtitle == null || subtitle.IsDeleted == true)
            {
                _logger.LogWarning("Subtitle not found or already deleted. Id: {SubtitleId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<SubtitleDto>(subtitle);
            subtitle.IsDeleted = true;
            subtitle.DeletedAt = DateTime.UtcNow;
            subtitle.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Subtitle, object>(
                tableName: "Subtitles",
                recordId: subtitle.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Subtitle soft?deleted. Id: {SubtitleId}", subtitle.Id);
            return true;
        }
    }
}

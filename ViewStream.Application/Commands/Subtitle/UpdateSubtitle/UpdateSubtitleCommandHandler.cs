using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.UpdateSubtitle
{
    using Subtitle = ViewStream.Domain.Entities.Subtitle;
    public class UpdateSubtitleCommandHandler : IRequestHandler<UpdateSubtitleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateSubtitleCommandHandler> _logger;

        public UpdateSubtitleCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateSubtitleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateSubtitleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating subtitle Id: {SubtitleId}", request.Id);

            var subtitle = await _unitOfWork.Subtitles.GetByIdAsync<long>(request.Id, cancellationToken);
            if (subtitle == null || subtitle.IsDeleted == true)
            {
                _logger.LogWarning("Subtitle not found or already deleted. Id: {SubtitleId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<SubtitleDto>(subtitle);
            _mapper.Map(request.Dto, subtitle);
            subtitle.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Subtitles.Update(subtitle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Subtitle, object>(
                tableName: "Subtitles",
                recordId: subtitle.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Subtitle updated. Id: {SubtitleId}", subtitle.Id);
            return true;
        }
    }
}

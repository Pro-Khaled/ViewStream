using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.CreateSubtitle
{
    using Subtitle = Domain.Entities.Subtitle;
    public class CreateSubtitleCommandHandler : IRequestHandler<CreateSubtitleCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateSubtitleCommandHandler> _logger;

        public CreateSubtitleCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateSubtitleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<long> Handle(CreateSubtitleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating subtitle for EpisodeId: {EpisodeId}, LanguageCode: {LanguageCode}",
                request.Dto.EpisodeId, request.Dto.LanguageCode);

            var subtitle = _mapper.Map<Subtitle>(request.Dto);
            subtitle.CreatedAt = DateTime.UtcNow;
            subtitle.IsDeleted = false;

            await _unitOfWork.Subtitles.AddAsync(subtitle, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Subtitle, object>(
                tableName: "Subtitles",
                recordId: subtitle.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Subtitle created with Id: {SubtitleId}", subtitle.Id);
            return subtitle.Id;
        }
    }
}

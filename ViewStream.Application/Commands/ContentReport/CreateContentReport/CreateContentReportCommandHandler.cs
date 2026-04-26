using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentReport.CreateContentReport
{
    using ContentReport = ViewStream.Domain.Entities.ContentReport;
    public class CreateContentReportCommandHandler : IRequestHandler<CreateContentReportCommand, ContentReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateContentReportCommandHandler> _logger;

        public CreateContentReportCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateContentReportCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<ContentReportDto> Handle(CreateContentReportCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            _logger.LogInformation("Submitting content report by ProfileId: {ProfileId}, ShowId: {ShowId}, EpisodeId: {EpisodeId}",
                request.ProfileId, dto.ShowId, dto.EpisodeId);

            if (!dto.ShowId.HasValue && !dto.EpisodeId.HasValue)
                throw new ArgumentException("Either ShowId or EpisodeId must be provided.");

            var existing = await _unitOfWork.ContentReports.FindAsync(
                r => r.ProfileId == request.ProfileId &&
                     r.ShowId == dto.ShowId &&
                     r.EpisodeId == dto.EpisodeId,
                cancellationToken: cancellationToken);

            if (existing.Any())
                throw new InvalidOperationException("You have already reported this content.");

            var report = new ContentReport
            {
                ProfileId = request.ProfileId,
                ShowId = dto.ShowId,
                EpisodeId = dto.EpisodeId,
                Reason = dto.Reason,
                Description = dto.Description,
                Status = "pending",
                ReportedAt = DateTime.UtcNow
            };

            await _unitOfWork.ContentReports.AddAsync(report, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<ContentReport, CreateContentReportDto>(
                tableName: "ContentReports",
                recordId: report.Id,
                action: "INSERT",
                newValues: dto,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Content report created with Id: {ReportId}", report.Id);

            var result = await _unitOfWork.ContentReports.FindAsync(
                r => r.Id == report.Id,
                include: q => q.Include(r => r.Profile)
                               .Include(r => r.Show)
                               .Include(r => r.Episode),
                cancellationToken: cancellationToken);

            return _mapper.Map<ContentReportDto>(result.First());
        }
    }
}

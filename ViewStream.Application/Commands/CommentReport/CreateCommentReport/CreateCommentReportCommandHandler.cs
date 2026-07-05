using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentReport.CreateCommentReport
{
    using CommentReport = ViewStream.Domain.Entities.CommentReport;
    public class CreateCommentReportCommandHandler : IRequestHandler<CreateCommentReportCommand, CommentReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly IReputationService _reputationService;
        private readonly IAdminNotificationHubClient _adminHubClient;
        private readonly ILogger<CreateCommentReportCommandHandler> _logger;

        public CreateCommentReportCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            IReputationService reputationService,
            IAdminNotificationHubClient adminHubClient,
            ILogger<CreateCommentReportCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _reputationService = reputationService;
            _adminHubClient = adminHubClient;
            _logger = logger;
        }

        public async Task<CommentReportDto> Handle(CreateCommentReportCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Submitting report for CommentId: {CommentId} by ProfileId: {ProfileId}",
                request.Dto.CommentId, request.ProfileId);

            // Prevent duplicate reports from same profile on same comment
            var existing = await _unitOfWork.CommentReports.FindAsync(
                r => r.CommentId == request.Dto.CommentId && r.ReportedByProfileId == request.ProfileId,
                cancellationToken: cancellationToken);

            if (existing.Any())
                throw new InvalidOperationException("You have already reported this comment.");

            var report = new CommentReport
            {
                CommentId = request.Dto.CommentId,
                ReportedByProfileId = request.ProfileId,
                Reason = request.Dto.Reason,
                Details = request.Dto.Details,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CommentReports.AddAsync(report, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<CommentReport, CreateCommentReportDto>(
                tableName: "CommentReports",
                recordId: report.Id,
                action: "INSERT",
                newValues: request.Dto,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Comment report created with Id: {ReportId}", report.Id);

            // Auto-hide at 5 reports threshold
            var reportCount = (await _unitOfWork.CommentReports.FindAsync(
                r => r.CommentId == request.Dto.CommentId)).Count();

            if (reportCount >= 5)
            {
                var comments = await _unitOfWork.EpisodeComments.FindAsync(
                    c => c.Id == request.Dto.CommentId);
                var targetComment = comments.FirstOrDefault();
                if (targetComment != null && !targetComment.IsHidden)
                {
                    targetComment.IsHidden = true;
                    _unitOfWork.EpisodeComments.Update(targetComment);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Deduct reputation from comment author
                    await _reputationService.AdjustReputationAsync(targetComment.Profile?.UserId ?? 0, -10, cancellationToken);

                    // Notify admins via SignalR
                    await _adminHubClient.SendModerationAlertAsync(
                        "Comment",
                        targetComment.Id,
                        reportCount,
                        $"Comment text: {targetComment.CommentText}",
                        cancellationToken);

                    _logger.LogWarning("Comment {CommentId} auto-hidden after {Count} reports",
                        request.Dto.CommentId, reportCount);
                }
            }

            var result = await _unitOfWork.CommentReports.FindAsync(
                r => r.Id == report.Id,
                include: q => q.Include(r => r.Comment).Include(r => r.ReportedByProfile),
                cancellationToken: cancellationToken);

            return _mapper.Map<CommentReportDto>(result.First());
        }
    }
}

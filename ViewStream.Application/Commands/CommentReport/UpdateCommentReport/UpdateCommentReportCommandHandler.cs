using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentReport.UpdateCommentReport
{
    using CommentReport = ViewStream.Domain.Entities.CommentReport;
    public class UpdateReportStatusCommandHandler : IRequestHandler<UpdateReportStatusCommand, CommentReportDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateReportStatusCommandHandler> _logger;

        public UpdateReportStatusCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateReportStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<CommentReportDto?> Handle(UpdateReportStatusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating status for ReportId: {ReportId} to {Status} by UserId: {UserId}",
                request.ReportId, request.Dto.Status, request.ReviewedByUserId);

            var report = await _unitOfWork.CommentReports.GetByIdAsync<long>(request.ReportId, cancellationToken);
            if (report == null)
            {
                _logger.LogWarning("Report not found with Id: {ReportId}", request.ReportId);
                return null;
            }

            var oldValues = _mapper.Map<CommentReportDto>(report);
            report.Status = request.Dto.Status;
            report.ReviewedByUserId = request.ReviewedByUserId;
            report.ReviewedAt = DateTime.UtcNow;

            _unitOfWork.CommentReports.Update(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<CommentReport, object>(
                tableName: "CommentReports",
                recordId: report.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: new { report.Status, report.ReviewedByUserId, report.ReviewedAt },
                changedByUserId: request.ReviewedByUserId
            );

            _logger.LogInformation("Report status updated for Id: {ReportId}", report.Id);

            var result = await _unitOfWork.CommentReports.FindAsync(
                r => r.Id == report.Id,
                include: q => q.Include(r => r.Comment)
                               .Include(r => r.ReportedByProfile)
                               .Include(r => r.ReviewedByUser),
                cancellationToken: cancellationToken);

            return _mapper.Map<CommentReportDto>(result.First());
        }
    }
}

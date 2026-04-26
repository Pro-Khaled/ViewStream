using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentReport.UpdateContentReport
{
    using ContentReport = ViewStream.Domain.Entities.ContentReport;
    public class UpdateContentReportStatusCommandHandler : IRequestHandler<UpdateContentReportStatusCommand, ContentReportDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateContentReportStatusCommandHandler> _logger;

        public UpdateContentReportStatusCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdateContentReportStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<ContentReportDto?> Handle(UpdateContentReportStatusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating status for ContentReportId: {ReportId} to {Status} by UserId: {UserId}",
                request.ReportId, request.Dto.Status, request.ReviewedByUserId);

            var report = await _unitOfWork.ContentReports.GetByIdAsync<long>(request.ReportId, cancellationToken);
            if (report == null)
            {
                _logger.LogWarning("Content report not found with Id: {ReportId}", request.ReportId);
                return null;
            }

            var oldValues = _mapper.Map<ContentReportDto>(report);
            report.Status = request.Dto.Status;
            if (request.Dto.Status != "pending")
                report.ResolvedAt = DateTime.UtcNow;

            _unitOfWork.ContentReports.Update(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<ContentReport, object>(
                tableName: "ContentReports",
                recordId: report.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: new { report.Status, report.ResolvedAt },
                changedByUserId: request.ReviewedByUserId
            );

            _logger.LogInformation("Content report status updated for Id: {ReportId}", report.Id);

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

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentReport.UpdateCommentReport
{
    public class UpdateReportStatusCommandHandler : IRequestHandler<UpdateReportStatusCommand, CommentReportDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateReportStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CommentReportDto?> Handle(UpdateReportStatusCommand request, CancellationToken cancellationToken)
        {
            var report = await _unitOfWork.CommentReports.GetByIdAsync<long>(request.ReportId, cancellationToken);
            if (report == null) return null;

            report.Status = request.Dto.Status;
            report.ReviewedByUserId = request.ReviewedByUserId;
            report.ReviewedAt = DateTime.UtcNow;

            _unitOfWork.CommentReports.Update(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

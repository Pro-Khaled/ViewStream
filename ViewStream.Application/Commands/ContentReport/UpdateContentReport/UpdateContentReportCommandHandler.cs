using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentReport.UpdateContentReport
{
    public class UpdateContentReportStatusCommandHandler : IRequestHandler<UpdateContentReportStatusCommand, ContentReportDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateContentReportStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ContentReportDto?> Handle(UpdateContentReportStatusCommand request, CancellationToken cancellationToken)
        {
            var report = await _unitOfWork.ContentReports.GetByIdAsync<long>(request.ReportId, cancellationToken);
            if (report == null) return null;

            report.Status = request.Dto.Status;
            if (request.Dto.Status != "pending")
                report.ResolvedAt = DateTime.UtcNow;

            _unitOfWork.ContentReports.Update(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

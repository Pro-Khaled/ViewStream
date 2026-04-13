using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ContentReport.CreateContentReport
{
    using ContentReport = ViewStream.Domain.Entities.ContentReport;
    public class CreateContentReportCommandHandler : IRequestHandler<CreateContentReportCommand, ContentReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateContentReportCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ContentReportDto> Handle(CreateContentReportCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // Ensure at least one target is specified
            if (!dto.ShowId.HasValue && !dto.EpisodeId.HasValue)
                throw new ArgumentException("Either ShowId or EpisodeId must be provided.");

            // Prevent duplicate reports from same profile on same target
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

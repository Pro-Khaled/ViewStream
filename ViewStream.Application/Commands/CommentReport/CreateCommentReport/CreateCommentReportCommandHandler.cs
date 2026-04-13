using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.CommentReport.CreateCommentReport
{
    using CommentReport = ViewStream.Domain.Entities.CommentReport;
    public class CreateCommentReportCommandHandler : IRequestHandler<CreateCommentReportCommand, CommentReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCommentReportCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CommentReportDto> Handle(CreateCommentReportCommand request, CancellationToken cancellationToken)
        {
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

            var result = await _unitOfWork.CommentReports.FindAsync(
                r => r.Id == report.Id,
                include: q => q.Include(r => r.Comment).Include(r => r.ReportedByProfile),
                cancellationToken: cancellationToken);

            return _mapper.Map<CommentReportDto>(result.First());
        }
    }
}

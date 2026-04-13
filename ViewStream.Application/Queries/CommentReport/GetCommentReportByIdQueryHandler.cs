using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.CommentReport
{

    public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, CommentReportDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReportByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CommentReportDto?> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            var reports = await _unitOfWork.CommentReports.FindAsync(
                r => r.Id == request.Id,
                include: q => q.Include(r => r.Comment)
                               .Include(r => r.ReportedByProfile)
                               .Include(r => r.ReviewedByUser),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var report = reports.FirstOrDefault();
            return report == null ? null : _mapper.Map<CommentReportDto>(report);
        }
    }
}

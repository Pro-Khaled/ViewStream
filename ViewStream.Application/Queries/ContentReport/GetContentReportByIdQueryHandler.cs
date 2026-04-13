using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ContentReport
{
    public class GetContentReportByIdQueryHandler : IRequestHandler<GetContentReportByIdQuery, ContentReportDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetContentReportByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ContentReportDto?> Handle(GetContentReportByIdQuery request, CancellationToken cancellationToken)
        {
            var reports = await _unitOfWork.ContentReports.FindAsync(
                r => r.Id == request.Id,
                include: q => q.Include(r => r.Profile)
                               .Include(r => r.Show)
                               .Include(r => r.Episode),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var report = reports.FirstOrDefault();
            return report == null ? null : _mapper.Map<ContentReportDto>(report);
        }
    }
}

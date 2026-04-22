using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.AuditLog
{
    public class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, AuditLogDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAuditLogByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AuditLogDto?> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
        {
            var logs = await _unitOfWork.AuditLogs.FindAsync(
                a => a.Id == request.Id,
                include: q => q.Include(a => a.ChangedByUser),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var log = logs.FirstOrDefault();
            return log == null ? null : _mapper.Map<AuditLogDto>(log);
        }
    }
}

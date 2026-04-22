using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.AuditLog.CreateAuditLog
{
    using AuditLog = ViewStream.Domain.Entities.AuditLog;
    public class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, AuditLogDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateAuditLogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AuditLogDto> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
        {
            var log = _mapper.Map<AuditLog>(request.Dto);
            log.ChangedAt = DateTime.UtcNow;

            await _unitOfWork.AuditLogs.AddAsync(log, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.AuditLogs.FindAsync(
                a => a.Id == log.Id,
                include: q => q.Include(a => a.ChangedByUser),
                cancellationToken: cancellationToken);

            return _mapper.Map<AuditLogDto>(result.First());
        }
    }
}

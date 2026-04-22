using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ErrorLog.CreateErrorLog
{
    using ErrorLog = ViewStream.Domain.Entities.ErrorLog;
    public class CreateErrorLogCommandHandler : IRequestHandler<CreateErrorLogCommand, ErrorLogDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateErrorLogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ErrorLogDto> Handle(CreateErrorLogCommand request, CancellationToken cancellationToken)
        {
            var log = _mapper.Map<ErrorLog>(request.Dto);
            log.OccurredAt = DateTime.UtcNow;

            await _unitOfWork.ErrorLogs.AddAsync(log, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.ErrorLogs.FindAsync(
                e => e.Id == log.Id,
                include: q => q.Include(e => e.User),
                cancellationToken: cancellationToken);

            return _mapper.Map<ErrorLogDto>(result.First());
        }
    }
}

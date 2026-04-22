using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ErrorLog
{
    public class GetErrorLogByIdQueryHandler : IRequestHandler<GetErrorLogByIdQuery, ErrorLogDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetErrorLogByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ErrorLogDto?> Handle(GetErrorLogByIdQuery request, CancellationToken cancellationToken)
        {
            var logs = await _unitOfWork.ErrorLogs.FindAsync(
                e => e.Id == request.Id,
                include: q => q.Include(e => e.User),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var log = logs.FirstOrDefault();
            return log == null ? null : _mapper.Map<ErrorLogDto>(log);
        }
    }
}

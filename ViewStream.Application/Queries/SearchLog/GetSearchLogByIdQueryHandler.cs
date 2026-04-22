using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.SearchLog
{
    public class GetSearchLogByIdQueryHandler : IRequestHandler<GetSearchLogByIdQuery, SearchLogDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSearchLogByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SearchLogDto?> Handle(GetSearchLogByIdQuery request, CancellationToken cancellationToken)
        {
            var logs = await _unitOfWork.SearchLogs.FindAsync(
                s => s.Id == request.Id,
                include: q => q.Include(s => s.Profile).Include(s => s.ClickedShow),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var log = logs.FirstOrDefault();
            return log == null ? null : _mapper.Map<SearchLogDto>(log);
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.SearchLog.CreateSearchLog
{
    using SearchLog = ViewStream.Domain.Entities.SearchLog;
    public class CreateSearchLogCommandHandler : IRequestHandler<CreateSearchLogCommand, SearchLogDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateSearchLogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SearchLogDto> Handle(CreateSearchLogCommand request, CancellationToken cancellationToken)
        {
            var log = new SearchLog
            {
                ProfileId = request.ProfileId,
                Query = request.Dto.Query,
                ResultsCount = request.Dto.ResultsCount,
                ClickedShowId = request.Dto.ClickedShowId,
                SearchAt = DateTime.UtcNow
            };

            await _unitOfWork.SearchLogs.AddAsync(log, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.SearchLogs.FindAsync(
                s => s.Id == log.Id,
                include: q => q.Include(s => s.Profile).Include(s => s.ClickedShow),
                cancellationToken: cancellationToken);

            return _mapper.Map<SearchLogDto>(result.First());
        }
    }
}

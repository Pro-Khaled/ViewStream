using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Season
{
    public class GetSeasonsByShowQueryHandler : IRequestHandler<GetSeasonsByShowQuery, List<SeasonListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSeasonsByShowQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<SeasonListItemDto>> Handle(GetSeasonsByShowQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Seasons.GetQueryable()
                .Where(s => s.ShowId == request.ShowId);

            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            var seasons = await query
                .OrderBy(s => s.SeasonNumber)
                .Include(s => s.Episodes.Where(e => e.IsDeleted != true))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<SeasonListItemDto>>(seasons);
        }
    }
}

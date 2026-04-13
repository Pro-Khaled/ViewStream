using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Episode
{
    public class GetEpisodesBySeasonQueryHandler : IRequestHandler<GetEpisodesBySeasonQuery, List<EpisodeListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetEpisodesBySeasonQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<EpisodeListItemDto>> Handle(GetEpisodesBySeasonQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Episodes.GetQueryable()
                .Where(e => e.SeasonId == request.SeasonId);

            if (!request.IncludeDeleted)
                query = query.Where(e => e.IsDeleted != true);

            var episodes = await query
                .OrderBy(e => e.EpisodeNumber)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<EpisodeListItemDto>>(episodes);
        }
    }
}

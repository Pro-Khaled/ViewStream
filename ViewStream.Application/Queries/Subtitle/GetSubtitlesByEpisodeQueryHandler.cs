using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Subtitle
{
    public class GetSubtitlesByEpisodeQueryHandler : IRequestHandler<GetSubtitlesByEpisodeQuery, List<SubtitleListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSubtitlesByEpisodeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<SubtitleListItemDto>> Handle(GetSubtitlesByEpisodeQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Subtitles.GetQueryable()
                .Where(s => s.EpisodeId == request.EpisodeId);

            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            var subtitles = await query
                .OrderBy(s => s.LanguageCode)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<SubtitleListItemDto>>(subtitles);
        }
    }
}

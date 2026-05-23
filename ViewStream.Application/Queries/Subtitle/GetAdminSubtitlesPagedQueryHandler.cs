using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Subtitle
{
    public class GetAdminSubtitlesPagedQueryHandler : IRequestHandler<GetAdminSubtitlesPagedQuery, PagedResult<AdminSubtitleListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminSubtitlesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminSubtitleListItemDto>> Handle(GetAdminSubtitlesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Subtitles.GetQueryable()
                .AsNoTracking();

            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            if (request.EpisodeId.HasValue)
                query = query.Where(s => s.EpisodeId == request.EpisodeId.Value);

            if (!string.IsNullOrWhiteSpace(request.LanguageCode))
                query = query.Where(s => s.LanguageCode == request.LanguageCode);

            if (request.IsCc.HasValue)
                query = query.Where(s => s.IsCc == request.IsCc.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(s =>
                    s.LanguageCode.Contains(term) ||
                    (s.Episode != null && s.Episode.Title.Contains(term)));
            }

            if (request.CreatedFrom.HasValue)
                query = query.Where(s => s.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(s => s.CreatedAt <= request.CreatedTo.Value);

            if (request.UpdatedFrom.HasValue)
                query = query.Where(s => s.UpdatedAt >= request.UpdatedFrom.Value);
            if (request.UpdatedTo.HasValue)
                query = query.Where(s => s.UpdatedAt <= request.UpdatedTo.Value);

            if (request.DeletedFrom.HasValue)
                query = query.Where(s => s.DeletedAt >= request.DeletedFrom.Value);
            if (request.DeletedTo.HasValue)
                query = query.Where(s => s.DeletedAt <= request.DeletedTo.Value);

            var projected = query.ProjectTo<AdminSubtitleListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(s => s.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminSubtitleListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}

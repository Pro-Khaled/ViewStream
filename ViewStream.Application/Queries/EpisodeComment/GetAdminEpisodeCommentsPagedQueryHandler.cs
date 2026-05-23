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

namespace ViewStream.Application.Queries.EpisodeComment
{
    public class GetAdminEpisodeCommentsPagedQueryHandler : IRequestHandler<GetAdminEpisodeCommentsPagedQuery, PagedResult<AdminEpisodeCommentListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminEpisodeCommentsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminEpisodeCommentListItemDto>> Handle(GetAdminEpisodeCommentsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.EpisodeComments.GetQueryable()
                .AsNoTracking();

            if (!request.IncludeDeleted)
                query = query.Where(ec => ec.IsDeleted != true);

            if (request.CreatedFrom.HasValue)
                query = query.Where(ec => ec.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(ec => ec.CreatedAt <= request.CreatedTo.Value);

            if (request.UpdatedFrom.HasValue)
                query = query.Where(ec => ec.UpdatedAt >= request.UpdatedFrom.Value);
            if (request.UpdatedTo.HasValue)
                query = query.Where(ec => ec.UpdatedAt <= request.UpdatedTo.Value);

            if (request.DeletedFrom.HasValue)
                query = query.Where(ec => ec.DeletedAt >= request.DeletedFrom.Value);
            if (request.DeletedTo.HasValue)
                query = query.Where(ec => ec.DeletedAt <= request.DeletedTo.Value);

            if (request.EpisodeId.HasValue)
                query = query.Where(ec => ec.EpisodeId == request.EpisodeId.Value);

            if (request.ProfileId.HasValue)
                query = query.Where(ec => ec.ProfileId == request.ProfileId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(ec => ec.CommentText.Contains(request.SearchTerm));

            var projected = query.ProjectTo<AdminEpisodeCommentListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(ec => ec.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminEpisodeCommentListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}

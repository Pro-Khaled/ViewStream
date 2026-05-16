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

namespace ViewStream.Application.Queries.CommentLike
{
    public class GetAdminCommentLikesPagedQueryHandler : IRequestHandler<GetAdminCommentLikesPagedQuery, PagedResult<CommentLikeListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminCommentLikesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<CommentLikeListItemDto>> Handle(GetAdminCommentLikesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.CommentLikes.GetQueryable()
                .AsNoTracking();

            if (request.CommentId.HasValue)
                query = query.Where(c => c.CommentId == request.CommentId.Value);

            if (request.ProfileId.HasValue)
                query = query.Where(c => c.ProfileId == request.ProfileId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(c => c.Profile.Name.Contains(request.SearchTerm));

            var projected = query.ProjectTo<CommentLikeListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(c => c.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<CommentLikeListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}

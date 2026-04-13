using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.EpisodeComment
{
    public class GetRootCommentsPagedQueryHandler : IRequestHandler<GetRootCommentsPagedQuery, PagedResult<EpisodeCommentListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRootCommentsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<EpisodeCommentListItemDto>> Handle(GetRootCommentsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.EpisodeComments.GetQueryable()
                .Where(c => c.EpisodeId == request.EpisodeId && c.ParentCommentId == null && c.IsDeleted != true);

            var totalCount = await query.CountAsync(cancellationToken);

            var comments = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(c => c.Profile)
                .Include(c => c.CommentLikes)
                .Include(c => c.InverseParentComment.Where(r => r.IsDeleted != true))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<EpisodeCommentListItemDto>
            {
                Items = _mapper.Map<List<EpisodeCommentListItemDto>>(comments),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}

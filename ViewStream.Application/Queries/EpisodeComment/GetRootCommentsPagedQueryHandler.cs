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
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.EpisodeComment
{
    public class GetRootCommentsPagedQueryHandler : IRequestHandler<GetRootCommentsPagedQuery, PagedResult<EpisodeCommentListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBlockCheckService _blockCheckService;

        public GetRootCommentsPagedQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IBlockCheckService blockCheckService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _blockCheckService = blockCheckService;
        }

        public async Task<PagedResult<EpisodeCommentListItemDto>> Handle(GetRootCommentsPagedQuery request, CancellationToken cancellationToken)
        {
            var blockedUserIds = request.CurrentUserId.HasValue && request.CurrentUserId.Value != 0
                ? await _blockCheckService.GetBlockedUserIdsAsync(request.CurrentUserId.Value)
                : new List<long>();

            var query = _unitOfWork.EpisodeComments.GetQueryable()
                .Where(c => c.EpisodeId == request.EpisodeId && 
                            c.ParentCommentId == null && 
                            c.IsDeleted != true &&
                            c.IsHidden != true);

            if (blockedUserIds.Any())
            {
                // Exclude root comments from users blocked by the current user
                query = query.Where(c => !blockedUserIds.Contains(c.Profile.UserId));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var comments = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(c => c.Profile)
                .Include(c => c.CommentLikes)
                .Include(c => c.InverseParentComment.Where(r => r.IsDeleted != true && r.IsHidden != true))
                    .ThenInclude(r => r.Profile)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Filter out reply comments from blocked users in memory
            if (blockedUserIds.Any())
            {
                foreach (var comment in comments)
                {
                    if (comment.InverseParentComment != null)
                    {
                        comment.InverseParentComment = comment.InverseParentComment
                            .Where(r => !blockedUserIds.Contains(r.Profile.UserId))
                            .ToList();
                    }
                }
            }

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

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.EpisodeComment
{
    public class GetCommentWithRepliesQueryHandler : IRequestHandler<GetCommentWithRepliesQuery, EpisodeCommentDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBlockCheckService _blockCheckService;

        public GetCommentWithRepliesQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IBlockCheckService blockCheckService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _blockCheckService = blockCheckService;
        }

        public async Task<EpisodeCommentDto?> Handle(GetCommentWithRepliesQuery request, CancellationToken cancellationToken)
        {
            var blockedUserIds = request.CurrentUserId.HasValue && request.CurrentUserId.Value != 0
                ? await _blockCheckService.GetBlockedUserIdsAsync(request.CurrentUserId.Value)
                : new List<long>();

            var comment = await _unitOfWork.EpisodeComments.GetQueryable()
                .Where(c => c.Id == request.Id && c.IsDeleted != true && c.IsHidden != true)
                .Include(c => c.Profile)
                .Include(c => c.Episode)
                .Include(c => c.CommentLikes)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (comment == null) return null;

            // If the comment author is blocked, filter out the entire comment
            if (blockedUserIds.Contains(comment.Profile.UserId))
                return null;

            var dto = _mapper.Map<EpisodeCommentDto>(comment);
            await LoadRepliesRecursively(dto, request.MaxDepth, blockedUserIds, cancellationToken);
            return dto;
        }

        private async Task LoadRepliesRecursively(EpisodeCommentDto dto, int depth, List<long> blockedUserIds, CancellationToken cancellationToken)
        {
            if (depth <= 0) return;

            var replies = await _unitOfWork.EpisodeComments.FindAsync(
                c => c.ParentCommentId == dto.Id && c.IsDeleted != true && c.IsHidden != true && (!blockedUserIds.Any() || !blockedUserIds.Contains(c.Profile.UserId)),
                include: q => q.Include(c => c.Profile).Include(c => c.CommentLikes),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var replyDtos = _mapper.Map<List<EpisodeCommentDto>>(replies.OrderBy(c => c.CreatedAt));
            dto.Replies = replyDtos;

            foreach (var reply in replyDtos)
            {
                await LoadRepliesRecursively(reply, depth - 1, blockedUserIds, cancellationToken);
            }
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.EpisodeComment
{
    public class GetCommentWithRepliesQueryHandler : IRequestHandler<GetCommentWithRepliesQuery, EpisodeCommentDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCommentWithRepliesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EpisodeCommentDto?> Handle(GetCommentWithRepliesQuery request, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.EpisodeComments.GetQueryable()
                .Where(c => c.Id == request.Id && c.IsDeleted != true)
                .Include(c => c.Profile)
                .Include(c => c.Episode)
                .Include(c => c.CommentLikes)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (comment == null) return null;

            var dto = _mapper.Map<EpisodeCommentDto>(comment);
            await LoadRepliesRecursively(dto, request.MaxDepth, cancellationToken);
            return dto;
        }

        private async Task LoadRepliesRecursively(EpisodeCommentDto dto, int depth, CancellationToken cancellationToken)
        {
            if (depth <= 0) return;

            var replies = await _unitOfWork.EpisodeComments.FindAsync(
                c => c.ParentCommentId == dto.Id && c.IsDeleted != true,
                include: q => q.Include(c => c.Profile).Include(c => c.CommentLikes),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var replyDtos = _mapper.Map<List<EpisodeCommentDto>>(replies.OrderBy(c => c.CreatedAt));
            dto.Replies = replyDtos;

            foreach (var reply in replyDtos)
            {
                await LoadRepliesRecursively(reply, depth - 1, cancellationToken);
            }
        }
    }
}

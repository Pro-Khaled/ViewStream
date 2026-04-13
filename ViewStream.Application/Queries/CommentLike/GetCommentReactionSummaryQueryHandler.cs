using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.CommentLike
{
    public class GetCommentReactionSummaryQueryHandler : IRequestHandler<GetCommentReactionSummaryQuery, CommentReactionSummaryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCommentReactionSummaryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CommentReactionSummaryDto> Handle(GetCommentReactionSummaryQuery request, CancellationToken cancellationToken)
        {
            var reactions = await _unitOfWork.CommentLikes.FindAsync(
                cl => cl.CommentId == request.CommentId,
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var reactionList = reactions.ToList();
            var summary = new CommentReactionSummaryDto
            {
                CommentId = request.CommentId,
                TotalReactions = reactionList.Count,
                ReactionCounts = reactionList
                    .GroupBy(r => r.ReactionType ?? "like")
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            if (request.CurrentProfileId.HasValue)
            {
                var userReaction = reactionList.FirstOrDefault(r => r.ProfileId == request.CurrentProfileId.Value);
                summary.CurrentUserReaction = userReaction?.ReactionType;
            }

            return summary;
        }
    }
}

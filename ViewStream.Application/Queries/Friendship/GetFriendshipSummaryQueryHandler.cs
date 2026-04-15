using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Friendship
{
    public class GetFriendshipSummaryQueryHandler : IRequestHandler<GetFriendshipSummaryQuery, FriendshipSummaryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFriendshipSummaryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FriendshipSummaryDto> Handle(GetFriendshipSummaryQuery request, CancellationToken cancellationToken)
        {
            var allUserFriendships = await _unitOfWork.Friendships.FindAsync(
                f => f.UserId == request.UserId || f.FriendId == request.UserId,
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var list = allUserFriendships.ToList();
            return new FriendshipSummaryDto
            {
                UserId = request.UserId,
                FriendCount = list.Count(f => f.Status == "accepted" && f.UserId == request.UserId),
                PendingSentCount = list.Count(f => f.Status == "pending" && f.UserId == request.UserId),
                PendingReceivedCount = list.Count(f => f.Status == "pending" && f.FriendId == request.UserId),
                BlockedCount = list.Count(f => f.Status == "blocked" && f.UserId == request.UserId)
            };
        }
    }
}

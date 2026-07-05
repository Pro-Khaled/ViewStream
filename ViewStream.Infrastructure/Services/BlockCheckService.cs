using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Services
{
    /// <summary>
    /// Checks block status between users using the Friendship table.
    /// A "blocked" status in either direction means the relationship is blocked.
    /// </summary>
    public class BlockCheckService : IBlockCheckService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BlockCheckService> _logger;

        public BlockCheckService(IUnitOfWork unitOfWork, ILogger<BlockCheckService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> IsBlockedAsync(long userId, long targetUserId)
        {
            var blocked = await _unitOfWork.Friendships.AnyAsync(
                f => ((f.UserId == userId && f.FriendId == targetUserId) ||
                      (f.UserId == targetUserId && f.FriendId == userId)) &&
                     f.Status == "blocked");
            return blocked;
        }

        public async Task<List<long>> GetBlockedUserIdsAsync(long userId)
        {
            var blockedRelations = await _unitOfWork.Friendships.FindAsync(
                f => (f.UserId == userId || f.FriendId == userId) && f.Status == "blocked",
                asNoTracking: true);

            return blockedRelations
                .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
                .Distinct()
                .ToList();
        }
    }
}

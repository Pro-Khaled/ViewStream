using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;
using User = ViewStream.Domain.Entities.User;

namespace ViewStream.Application.Queries.User
{
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, List<UserPublicSearchResultDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlockCheckService _blockCheckService;
        private readonly ILogger<SearchUsersQueryHandler> _logger;

        public SearchUsersQueryHandler(
            IUnitOfWork unitOfWork,
            IBlockCheckService blockCheckService,
            ILogger<SearchUsersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _blockCheckService = blockCheckService;
            _logger = logger;
        }

        public async Task<List<UserPublicSearchResultDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Searching users with query: {Q}", request.Q);

            var term = request.Q.Trim().ToLower();

            // Fetch list of user IDs that the current user has blocked
            var blockedUserIds = request.CurrentUserId.HasValue
                ? await _blockCheckService.GetBlockedUserIdsAsync(request.CurrentUserId.Value)
                : new List<long>();

            var matches = await _unitOfWork.Users.FindAsync(
                u => !u.IsDeleted && u.IsActive &&
                     (!request.CurrentUserId.HasValue || (u.Id != request.CurrentUserId.Value && !blockedUserIds.Contains(u.Id))) &&
                     (u.FullName!.ToLower().Contains(term) || u.Email!.ToLower().Contains(term)),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return matches
                .Take(request.Limit)
                .Select(u => new UserPublicSearchResultDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email!,
                    AvatarUrl = null  // AvatarUrl lives on Profile, not User — omit for now
                })
                .ToList();
        }
    }
}

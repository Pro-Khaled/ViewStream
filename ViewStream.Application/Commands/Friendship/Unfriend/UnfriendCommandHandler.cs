using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Friendship.Unfriend
{
    using Friendship = ViewStream.Domain.Entities.Friendship;
    public class UnfriendCommandHandler : IRequestHandler<UnfriendCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UnfriendCommandHandler> _logger;

        public UnfriendCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UnfriendCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UnfriendCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} unfriending user {FriendId}", request.UserId, request.FriendId);

            var friendships = await _unitOfWork.Friendships.FindAsync(
                f => (f.UserId == request.UserId && f.FriendId == request.FriendId) ||
                     (f.UserId == request.FriendId && f.FriendId == request.UserId),
                cancellationToken: cancellationToken);

            var toDelete = friendships.Where(f => f.Status == "accepted" || f.Status == "pending").ToList();
            if (!toDelete.Any())
            {
                _logger.LogWarning("No active friendship found between {UserId} and {FriendId}", request.UserId, request.FriendId);
                return false;
            }

            var oldValues = _mapper.Map<List<FriendshipDto>>(toDelete);

            foreach (var f in toDelete)
            {
                _unitOfWork.Friendships.Delete(f);

                _auditContext.SetAudit<Friendship, object>(
                    tableName: "Friendships",
                    recordId: f.UserId.GetHashCode() ^ f.FriendId.GetHashCode(),
                    action: "DELETE",
                    oldValues: new { f.UserId, f.FriendId, f.Status },
                    changedByUserId: request.ActorUserId
                );
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} unfriended user {FriendId}", request.UserId, request.FriendId);
            return true;
        }
    }

}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Friendship.RespondToFriendRequest
{
    using Friendship = Domain.Entities.Friendship;

    public class RespondToFriendRequestCommandHandler : IRequestHandler<RespondToFriendRequestCommand, FriendshipDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RespondToFriendRequestCommandHandler> _logger;

        public RespondToFriendRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RespondToFriendRequestCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<FriendshipDto?> Handle(RespondToFriendRequestCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} responding to friend request from {FriendId} with status {Status}",
                request.UserId, request.FriendId, request.Dto.Status);

            var friendships = await _unitOfWork.Friendships.FindAsync(
                f => f.UserId == request.FriendId && f.FriendId == request.UserId && f.Status == "pending",
                cancellationToken: cancellationToken);

            var friendship = friendships.FirstOrDefault();
            if (friendship == null)
            {
                _logger.LogWarning("Pending request not found from {FriendId} to {UserId}", request.FriendId, request.UserId);
                return null;
            }

            var oldValues = new { friendship.Status };
            friendship.Status = request.Dto.Status;
            friendship.UpdatedAt = DateTime.UtcNow;

            if (request.Dto.Status == "accepted")
            {
                var reverse = await _unitOfWork.Friendships.FindAsync(
                    f => f.UserId == request.UserId && f.FriendId == request.FriendId,
                    cancellationToken: cancellationToken);
                if (!reverse.Any())
                {
                    var reverseFriendship = new Friendship
                    {
                        UserId = request.UserId,
                        FriendId = request.FriendId,
                        Status = "accepted",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Friendships.AddAsync(reverseFriendship, cancellationToken);
                }
            }

            _unitOfWork.Friendships.Update(friendship);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Friendship, object>(
                tableName: "Friendships",
                recordId: friendship.UserId.GetHashCode() ^ friendship.FriendId.GetHashCode(),
                action: "UPDATE",
                oldValues: oldValues,
                newValues: new { friendship.Status },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Friend request from {FriendId} to {UserId} set to {Status}",
                request.FriendId, request.UserId, request.Dto.Status);

            var result = await _unitOfWork.Friendships.FindAsync(
                f => f.UserId == friendship.UserId && f.FriendId == friendship.FriendId,
                include: q => q.Include(f => f.User).Include(f => f.Friend),
                cancellationToken: cancellationToken);

            return _mapper.Map<FriendshipDto>(result.First());
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Friendship.SendFriendRequest
{
    using Friendship = Domain.Entities.Friendship;
    public class SendFriendRequestCommandHandler : IRequestHandler<SendFriendRequestCommand, FriendshipDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<SendFriendRequestCommandHandler> _logger;

        public SendFriendRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<SendFriendRequestCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<FriendshipDto> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} sending friend request to {FriendId}", request.UserId, request.Dto.FriendId);

            if (request.UserId == request.Dto.FriendId)
                throw new InvalidOperationException("You cannot send a friend request to yourself.");

            var existing = await _unitOfWork.Friendships.FindAsync(
                f => (f.UserId == request.UserId && f.FriendId == request.Dto.FriendId) ||
                     (f.UserId == request.Dto.FriendId && f.FriendId == request.UserId),
                cancellationToken: cancellationToken);

            var existingRelation = existing.FirstOrDefault();
            if (existingRelation != null)
            {
                if (existingRelation.Status == "blocked")
                    throw new InvalidOperationException("Cannot send request to a blocked user.");
                if (existingRelation.Status == "accepted")
                    throw new InvalidOperationException("You are already friends.");
                if (existingRelation.UserId == request.UserId && existingRelation.Status == "pending")
                    throw new InvalidOperationException("Friend request already sent.");
                if (existingRelation.FriendId == request.UserId && existingRelation.Status == "pending")
                    throw new InvalidOperationException("This user has already sent you a request. Accept it instead.");
            }

            var friendship = new Friendship
            {
                UserId = request.UserId,
                FriendId = request.Dto.FriendId,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Friendships.AddAsync(friendship, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Friendship, object>(
                tableName: "Friendships",
                recordId: friendship.UserId.GetHashCode() ^ friendship.FriendId.GetHashCode(),
                action: "INSERT",
                oldValues: null,
                newValues: new { friendship.UserId, friendship.FriendId, friendship.Status },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Friend request sent: User {UserId} -> Friend {FriendId}", request.UserId, request.Dto.FriendId);

            var result = await _unitOfWork.Friendships.FindAsync(
                f => f.UserId == friendship.UserId && f.FriendId == friendship.FriendId,
                include: q => q.Include(f => f.User).Include(f => f.Friend),
                cancellationToken: cancellationToken);

            return _mapper.Map<FriendshipDto>(result.First());
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Friendship.BlockUser
{
    using Friendship = Domain.Entities.Friendship;
    public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, FriendshipDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<BlockUserCommandHandler> _logger;

        public BlockUserCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<BlockUserCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<FriendshipDto> Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} blocking user {FriendId}", request.UserId, request.FriendId);

            if (request.UserId == request.FriendId)
                throw new InvalidOperationException("You cannot block yourself.");

            var existing = await _unitOfWork.Friendships.FindAsync(
                f => (f.UserId == request.UserId && f.FriendId == request.FriendId) ||
                     (f.UserId == request.FriendId && f.FriendId == request.UserId),
                cancellationToken: cancellationToken);

            var friendship = existing.FirstOrDefault();
            bool isNew = false;
            string oldStatus = friendship?.Status ?? "none";

            if (friendship != null)
            {
                friendship.Status = "blocked";
                friendship.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Friendships.Update(friendship);
            }
            else
            {
                isNew = true;
                friendship = new Friendship
                {
                    UserId = request.UserId,
                    FriendId = request.FriendId,
                    Status = "blocked",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Friendships.AddAsync(friendship, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Friendship, object>(
                tableName: "Friendships",
                recordId: friendship.UserId.GetHashCode() ^ friendship.FriendId.GetHashCode(),
                action: isNew ? "INSERT" : "UPDATE",
                oldValues: isNew ? null : new { oldStatus },
                newValues: new { friendship.UserId, friendship.FriendId, friendship.Status },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("User {UserId} blocked user {FriendId}", request.UserId, request.FriendId);

            var result = await _unitOfWork.Friendships.FindAsync(
                f => f.UserId == friendship.UserId && f.FriendId == friendship.FriendId,
                include: q => q.Include(f => f.User).Include(f => f.Friend),
                cancellationToken: cancellationToken);

            return _mapper.Map<FriendshipDto>(result.First());
        }
    }
}

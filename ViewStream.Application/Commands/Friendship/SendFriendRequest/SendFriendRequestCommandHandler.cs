using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Friendship.SendFriendRequest
{
    using Friendship = Domain.Entities.Friendship;
    public class SendFriendRequestCommandHandler : IRequestHandler<SendFriendRequestCommand, FriendshipDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SendFriendRequestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FriendshipDto> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == request.Dto.FriendId)
                throw new InvalidOperationException("You cannot send a friend request to yourself.");

            // Check existing relationship
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

            var result = await _unitOfWork.Friendships.FindAsync(
                f => f.UserId == friendship.UserId && f.FriendId == friendship.FriendId,
                include: q => q.Include(f => f.User).Include(f => f.Friend),
                cancellationToken: cancellationToken);

            return _mapper.Map<FriendshipDto>(result.First());
        }
    }
}

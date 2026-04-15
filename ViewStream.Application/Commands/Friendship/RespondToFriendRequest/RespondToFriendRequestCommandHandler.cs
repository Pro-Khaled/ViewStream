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

namespace ViewStream.Application.Commands.Friendship.RespondToFriendRequest
{
    using Friendship = Domain.Entities.Friendship;

    public class RespondToFriendRequestCommandHandler : IRequestHandler<RespondToFriendRequestCommand, FriendshipDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RespondToFriendRequestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FriendshipDto?> Handle(RespondToFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var friendships = await _unitOfWork.Friendships.FindAsync(
                f => f.UserId == request.FriendId && f.FriendId == request.UserId && f.Status == "pending",
                cancellationToken: cancellationToken);

            var friendship = friendships.FirstOrDefault();
            if (friendship == null) return null;

            if (request.Dto.Status == "accepted")
            {
                friendship.Status = "accepted";
                friendship.UpdatedAt = DateTime.UtcNow;

                // Optionally create reverse accepted entry for symmetry (not required but common)
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
            else if (request.Dto.Status == "blocked")
            {
                friendship.Status = "blocked";
                friendship.UpdatedAt = DateTime.UtcNow;
            }

            _unitOfWork.Friendships.Update(friendship);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.Friendships.FindAsync(
                f => f.UserId == friendship.UserId && f.FriendId == friendship.FriendId,
                include: q => q.Include(f => f.User).Include(f => f.Friend),
                cancellationToken: cancellationToken);

            return _mapper.Map<FriendshipDto>(result.First());
        }
    }
}

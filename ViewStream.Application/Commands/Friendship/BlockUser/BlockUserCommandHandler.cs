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

namespace ViewStream.Application.Commands.Friendship.BlockUser
{
    using Friendship = Domain.Entities.Friendship;
    public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, FriendshipDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BlockUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FriendshipDto> Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == request.FriendId)
                throw new InvalidOperationException("You cannot block yourself.");

            var existing = await _unitOfWork.Friendships.FindAsync(
                f => (f.UserId == request.UserId && f.FriendId == request.FriendId) ||
                     (f.UserId == request.FriendId && f.FriendId == request.UserId),
                cancellationToken: cancellationToken);

            var friendship = existing.FirstOrDefault();
            if (friendship != null)
            {
                friendship.Status = "blocked";
                friendship.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Friendships.Update(friendship);
            }
            else
            {
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

            var result = await _unitOfWork.Friendships.FindAsync(
                f => f.UserId == friendship.UserId && f.FriendId == friendship.FriendId,
                include: q => q.Include(f => f.User).Include(f => f.Friend),
                cancellationToken: cancellationToken);

            return _mapper.Map<FriendshipDto>(result.First());
        }
    }
}

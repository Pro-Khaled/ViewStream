using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Friendship.Unfriend
{
    public class UnfriendCommandHandler : IRequestHandler<UnfriendCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnfriendCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UnfriendCommand request, CancellationToken cancellationToken)
        {
            var friendships = await _unitOfWork.Friendships.FindAsync(
                f => (f.UserId == request.UserId && f.FriendId == request.FriendId) ||
                     (f.UserId == request.FriendId && f.FriendId == request.UserId),
                cancellationToken: cancellationToken);

            var toDelete = friendships.Where(f => f.Status == "accepted" || f.Status == "pending").ToList();
            if (!toDelete.Any()) return false;

            foreach (var f in toDelete)
                _unitOfWork.Friendships.Delete(f);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}

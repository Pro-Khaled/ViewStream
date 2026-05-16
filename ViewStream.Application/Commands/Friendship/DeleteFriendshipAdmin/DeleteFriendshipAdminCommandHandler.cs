using AutoMapper;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Exceptions;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Friendship.DeleteFriendshipAdmin
{
    public class DeleteFriendshipAdminCommandHandler : IRequestHandler<DeleteFriendshipAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteFriendshipAdminCommandHandler> _logger;

        public DeleteFriendshipAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteFriendshipAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteFriendshipAdminCommand request, CancellationToken cancellationToken)
        {
            var friendship = await _unitOfWork.Friendships.GetQueryable()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.FriendId == request.FriendId, cancellationToken);
            
            if (friendship == null)
            {
                throw new NotFoundException("Friendship", $"{request.UserId}/{request.FriendId}");
            }

            _unitOfWork.Friendships.Delete(friendship);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Friendship removed by admin. UserId: {UserId}, FriendId: {FriendId}", request.UserId, request.FriendId);
            return true;
        }
    }
}


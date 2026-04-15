using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Friendship.RespondToFriendRequest
{
    public record RespondToFriendRequestCommand(long UserId, long FriendId, UpdateFriendshipStatusDto Dto) : IRequest<FriendshipDto?>;

}

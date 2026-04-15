using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Friendship.SendFriendRequest
{
    public record SendFriendRequestCommand(long UserId, FriendRequestDto Dto) : IRequest<FriendshipDto>;
}

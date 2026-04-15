using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Friendship.BlockUser
{
    public record BlockUserCommand(long UserId, long FriendId) : IRequest<FriendshipDto>;

}

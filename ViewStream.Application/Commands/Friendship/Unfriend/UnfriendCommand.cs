using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.Friendship.Unfriend
{
    public record UnfriendCommand(long UserId, long FriendId) : IRequest<bool>;

}

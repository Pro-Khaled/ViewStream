using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Friendship
{
    public record GetFriendsQuery(long UserId, string? Status = "accepted") : IRequest<List<FriendshipListItemDto>>;

}

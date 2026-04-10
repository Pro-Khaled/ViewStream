using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.User.BlockUser
{
    // Admin: Block user
    public record BlockUserCommand(long UserId, BlockUserDto Dto, long AdminUserId) : IRequest<bool>;
}

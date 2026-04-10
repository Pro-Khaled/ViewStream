using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.User.AdminUpdateUser
{
    // Admin: Update user
    public record AdminUpdateUserCommand(long UserId, AdminUpdateUserDto Dto) : IRequest<bool>;
}

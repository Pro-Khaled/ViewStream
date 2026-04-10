using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.User.UpdateProfile
{
    // Update own profile
    public record UpdateProfileCommand(long UserId, UpdateProfileDto Dto) : IRequest<bool>;
}

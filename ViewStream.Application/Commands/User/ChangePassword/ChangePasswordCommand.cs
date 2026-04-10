using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.User.ChangePassword
{
    // Change password
    public record ChangePasswordCommand(long UserId, ChangePasswordDto Dto) : IRequest<IdentityResult>;
}

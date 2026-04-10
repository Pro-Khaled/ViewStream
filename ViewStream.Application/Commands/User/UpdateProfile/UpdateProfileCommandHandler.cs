using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.User.UpdateProfile
{
    using User = ViewStream.Domain.Entities.User;

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public UpdateProfileCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted)
                return false;

            user.FullName = request.Dto.FullName;
            user.PhoneNumber = request.Dto.PhoneNumber;
            user.CountryCode = request.Dto.CountryCode;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Identity;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.User.DeleteUser
{

    using User = ViewStream.Domain.Entities.User;

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public DeleteUserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return false;

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.IsActive = false; // Also deactivate
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}

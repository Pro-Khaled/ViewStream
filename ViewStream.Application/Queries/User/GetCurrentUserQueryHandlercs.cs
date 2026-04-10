using MediatR;
using Microsoft.AspNetCore.Identity;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.User
{
    using User = ViewStream.Domain.Entities.User;


    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto?>
    {
        private readonly UserManager<User> _userManager;

        public GetCurrentUserQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted || !user.IsActive)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                CountryCode = user.CountryCode,
                IsActive = user.IsActive,
                IsBlocked = user.IsBlocked,
                BlockedReason = user.BlockedReason,
                BlockedUntil = user.BlockedUntil,
                CreatedAt = user.CreatedAt,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            };
        }
    }
}

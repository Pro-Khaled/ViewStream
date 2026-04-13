using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Profile.UpdateProfile
{
    public record UpdateProfileCommand(long Id, long UserId, UpdateProfileDto Dto) : IRequest<ProfileDto?>;

}

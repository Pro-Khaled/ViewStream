using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Profile.CreateProfile
{
    public record CreateProfileCommand(long UserId, CreateProfileDto Dto) : IRequest<ProfileDto>;

}

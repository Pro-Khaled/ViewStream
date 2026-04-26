using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Profile.SwitchActiveProfile
{
    public record SwitchActiveProfileCommand(long UserId, long ProfileId) : IRequest<SwitchProfileResponseDto?>;

}

using MediatR;
using ViewStream.Application.Common;

namespace ViewStream.Application.Commands.Profile.DeleteProfile
{
    public record DeleteProfileCommand(long Id, long UserId) : IRequest<bool>; // Soft delete

}

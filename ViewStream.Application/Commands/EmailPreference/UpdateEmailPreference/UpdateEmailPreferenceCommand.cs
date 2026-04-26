using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.EmailPreference.UpdateEmailPreference
{
    public record UpdateEmailPreferenceCommand(long UserId, UpdateEmailPreferenceDto Dto, long ActorUserId)
        : IRequest<EmailPreferenceDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}

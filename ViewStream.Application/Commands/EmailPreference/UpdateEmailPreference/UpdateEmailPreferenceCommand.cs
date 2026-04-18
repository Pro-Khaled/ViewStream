using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.EmailPreference.UpdateEmailPreference
{
    public record UpdateEmailPreferenceCommand(long UserId, UpdateEmailPreferenceDto Dto) : IRequest<EmailPreferenceDto>;

}

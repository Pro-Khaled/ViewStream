using MediatR;

namespace ViewStream.Application.Commands.Role.DeleteRole
{
    public record DeleteRoleCommand(long Id) : IRequest<bool>;

}

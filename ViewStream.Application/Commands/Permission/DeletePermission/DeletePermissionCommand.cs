using MediatR;

namespace ViewStream.Application.Commands.Permission.DeletePermission
{
    public record DeletePermissionCommand(int Id) : IRequest<bool>;

}

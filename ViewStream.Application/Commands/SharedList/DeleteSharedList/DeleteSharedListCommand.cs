using MediatR;

namespace ViewStream.Application.Commands.SharedList.DeleteSharedList
{
    public record DeleteSharedListCommand(long Id, long OwnerProfileId) : IRequest<bool>;

}

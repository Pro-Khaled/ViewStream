using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.SharedList.CreateSharedList
{
    public record CreateSharedListCommand(long OwnerProfileId, CreateSharedListDto Dto) : IRequest<SharedListDto>;

}

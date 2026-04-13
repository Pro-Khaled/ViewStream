using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.SharedList.UpdateSharedList
{
    public record UpdateSharedListCommand(long Id, long OwnerProfileId, UpdateSharedListDto Dto) : IRequest<SharedListDto?>;

}

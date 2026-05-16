using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ItemVector.UpsertItemVector
{
    public record UpsertItemVectorCommand(long ShowId, CreateUpdateItemVectorDto Dto, long AdminUserId) : IRequest<bool>;
}

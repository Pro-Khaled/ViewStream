using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.SharedList
{
    public record GetSharedListByIdQuery(long Id, long? RequestingProfileId = null) : IRequest<SharedListDto?>;

}

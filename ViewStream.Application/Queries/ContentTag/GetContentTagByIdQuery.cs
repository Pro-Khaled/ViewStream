using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ContentTag
{
    public record GetContentTagByIdQuery(int Id) : IRequest<ContentTagDto?>;

}

using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ContentTag
{
    public record GetContentTagsByCategoryQuery(string Category) : IRequest<List<ContentTagListItemDto>>;
}

using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ContentTag.CreateContentTag
{
    public record CreateContentTagCommand(CreateContentTagDto Dto) : IRequest<int>;

}

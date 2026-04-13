using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ContentTag.UpdateContentTag
{
    public record UpdateContentTagCommand(int Id, UpdateContentTagDto Dto) : IRequest<bool>;

}
